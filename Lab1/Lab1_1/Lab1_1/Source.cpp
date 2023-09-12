#include <bitset>
#include <cstddef>
#include <iostream>
#include <fstream>
//#include <stdio.h>
//#include <conio.h>

using namespace std;

long get_filesize(string filename){
    ifstream file(filename, ios::binary | ios::ate);

    long result = file.tellg();

    file.close();
    
    return result;
}

// функция, реализующая работу ГОСТ 28147-89 в режиме простой замены
void easy_replacement(string filename_input, string filename_output, int status) {

    // uint8_t 8 бит
    // строки таблицы, что заменять
    uint8_t replacement_tab[8][16] = {
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF,
     0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF
    };

    // ключ, на что заменять
    unsigned long key[8] = {
     0x0123,
     0x4567,
     0x89AB,
     0xCDEF,
     0x0123,
     0x4567,
     0x89AB,
     0xCDEF
    };

    char N[4]; // 32-разрядный накопитель,
    unsigned long n1 = 0, n2 = 0, SUM232 = 0; // накопители N1, N2, и сумматор

    char b;

    // открываем файлы
    ifstream file_reader(filename_input, ios::in | ios::binary);
    ofstream file_writer(filename_output, ios::out | ios::binary);

    const long filesize = get_filesize(filename_input);

    // определим количество блоков
    float blokoff;
    blokoff = 8 * get_filesize(filename_input); // long 8 бит
    blokoff = blokoff / 64; // разбиваем на блоки по 62=4 бита

    cout <<endl<<  blokoff << endl;

    int block = blokoff;
    if (blokoff - block > 0) {
        block++;
    }

    cout << block << endl;

    int sh;
    if (filesize >= 4) {
        sh = 4;
    } else {
        sh = filesize;
    }

    int sh1 = 0;
    int flag = 0;

    // начнем считывание и преобразование блоков
    // присутствуют проверки на полноту блоков, чтобы считать только нужное количество бит
    for (int i = 0; i < block; i++) {

        // записываем в накопитель N1
        for (int j = 0; j < 4; j++) {
            *((uint8_t*)&N + j) = 0x00; //очищаем биты
        } 

        if ((sh1 + sh) < filesize) {
            file_reader.read(N, sh);
            sh1 += sh;
        } else {
            sh = filesize - sh1;
            file_reader.read(N, sh);
            flag = 1;
        }
        n1 = *((unsigned long*)&N);

        // записываем в накопитель N2
        for (int j = 0; j < 4; j++) {
            *((uint8_t*)&N + j) = 0x00; //очищаем биты
        }

        if ((sh1 + sh) < filesize) {
            file_reader.read(N, sh);
            sh1 += sh;
        } else {
            if (flag == 0) {
                sh = filesize - sh1;
                file_reader.read(N, sh1);
            }
        }
        n2 = *((unsigned long*)&N);

        // 32 цикла простой замены
        // ключ считываем в требуемом ГОСТом порядке
        int c = 0;
        for (int k = 0; k < 32; k++) {
            if (status) {
                if (k == 24) c = 7;
            }
            else {
                if (k == 8) c = 7;
            }

            // суммируем в сумматоре СМ1
            SUM232 = key[c] + n1;

            // заменяем по таблице замен
            uint8_t first_byte = 0, second_byte = 0, zam_symbol = 0;
            int n = 7;
            for (int q = 3; q >= 0; q--) {
                zam_symbol = *((uint8_t*)&SUM232 + q);
                first_byte = (zam_symbol & 0xF0) >> 4;
                second_byte = (zam_symbol & 0x0F);
                first_byte = replacement_tab[n][first_byte];
                n--;
                second_byte = replacement_tab[n][second_byte];
                n--;
                zam_symbol = (first_byte << 4) | second_byte;
                *((uint8_t*)&SUM232 + q) = zam_symbol;
            }

            SUM232 = (SUM232 << 11) | (SUM232 >> (sizeof(unsigned long) - 11)); // циклический сдвиг на 11
            SUM232 = n2 ^ SUM232; // складываем в сумматоре СМ2

            if (k < 31) {
                n2 = n1;
                n1 = SUM232;
            } else {
                // n1 сохраняет старое значение
                n2 = SUM232;
            }

            if (status) {
                if (k < 24) {
                    c++;
                    if (c > 7) c = 0;
                } else {
                    c--;
                    if (c < 0) c = 7;
                }
            } else {
                if (k < 8) {
                    c++;
                    if (c > 7) c = 0;
                } else {
                    c--;
                    if (c < 0) c = 7;
                }
            }            
        }


        // вывод результата в файл
        char sym_result;
        for (int j = 0; j < 4; j++) {
            sym_result = *((uint8_t*)&n1 + j);
            file_writer.write(&sym_result, 1);
            //fprintf(f_end, "%c", sym_rez);
        }
        for (int j = 0; j < 4; j++) {
            sym_result = *((uint8_t*)&n2 + j);
            file_writer.write(&sym_result, 1);
            //fprintf(f_end, "%c", sym_rez);
        }
    }

    file_writer.close();
    file_reader.close();
    
        
}

int main() {
    //cout << (sizeof(unsigned long)*4 - 11);
    easy_replacement("output.txt", "output1.txt", 0);
	return 0;
}