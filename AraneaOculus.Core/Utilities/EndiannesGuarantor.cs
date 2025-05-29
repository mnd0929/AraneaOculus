namespace AraneaOculus.Core.Utilities
{
    public class EndiannesGuarantor
    {
        public static void GuaranteeLittleEndian(byte[] data)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);
        }

        public static void GuaranteeBigEndian(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
        }
    }
}
