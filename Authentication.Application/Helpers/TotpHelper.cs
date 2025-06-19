using System.Security.Cryptography;
using System.Text;

public static class TotpHelper {

    public static bool VerifyCode(string secret, string code, int window = 1, int timeStep = 30) {
        long currentCounter = GetCurrentCounter(timeStep);

        for (int i = -window; i <= window; i++) {
            long counter = currentCounter + i;
            string expected = GenerateCode(secret, 6, timeStep, counter);
            if (expected == code)
                return true;
        }

        return false;
    }

    private static string GenerateCode(string secret, int digits, int timeStep, long counter) {
        var key = Encoding.UTF8.GetBytes(secret);

        byte[] counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);

        using var hmac = new HMACSHA1(key);
        byte[] hash = hmac.ComputeHash(counterBytes);

        int offset = hash[^1] & 0x0F;

        int binaryCode = ((hash[offset] & 0x7F) << 24)
                       | ((hash[offset + 1] & 0xFF) << 16)
                       | ((hash[offset + 2] & 0xFF) << 8)
                       | (hash[offset + 3] & 0xFF);

        int otp = binaryCode % (int)Math.Pow(10, digits);
        return otp.ToString(new string('0', digits));
    }

    private static long GetCurrentCounter(int timeStep = 30) {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return unixTime / timeStep;
    }

    public static string Base32Encode(byte[] data) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        StringBuilder result = new StringBuilder();
        int buffer = data[0], next = 1, bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length) {
            if (bitsLeft < 5) {
                if (next < data.Length) {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xFF;
                    bitsLeft += 8;
                } else {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int index = 0x1F & (buffer >> (bitsLeft - 5));
            bitsLeft -= 5;
            result.Append(chars[index]);
        }

        return result.ToString();
    }
}
