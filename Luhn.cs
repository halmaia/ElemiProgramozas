private static bool Luhn(string crd)
{
    int s = 0, i = 0;
    do
    {
        int c = (crd[i++] - '0') << 1;
        if (c > 9) s -= 9;
        s += c + crd[i] - '0';

    } while (++i is not 16);
    return s % 10 is 0;
}
