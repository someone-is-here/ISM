using Lab5;

Hasher3411.WriteHashIntoFile("input.txt", "hash3411_256.txt", Hasher3411.HashMode.Mode256);
Hasher3411.WriteHashIntoFile("input.txt", "hash3411_512.txt", Hasher3411.HashMode.Mode512);

HasherSHA1.WriteHashIntoFile("input.txt", "hashSha1.txt");