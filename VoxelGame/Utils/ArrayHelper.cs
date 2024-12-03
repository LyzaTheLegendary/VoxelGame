namespace Utils
{
    public static class ArrayHelper
    {
        public static byte[] ShiftByteArray(byte[] data, int shift)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();

            int shiftBits = Math.Abs(shift) % 8; // Ensure shift is within 0-7 bits
            int carryBits = 8 - shiftBits; // Bits to carry over between bytes
            bool isLeftShift = shift >= 0; // Determine shift direction

            byte[] shiftedData = new byte[data.Length];

            if (isLeftShift)
            {
                // Left shift
                for (int i = 0; i < data.Length; i++)
                {
                    shiftedData[i] = (byte)(data[i] << shiftBits);
                    if (i + 1 < data.Length)
                    {
                        shiftedData[i] |= (byte)(data[i + 1] >> carryBits);
                    }
                }
            }
            else
            {
                // Right shift
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    shiftedData[i] = (byte)(data[i] >> shiftBits);
                    if (i - 1 >= 0)
                    {
                        shiftedData[i] |= (byte)(data[i - 1] << carryBits);
                    }
                }
            }

            return shiftedData;
        }
    }
}
