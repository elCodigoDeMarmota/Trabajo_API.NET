using System.Security.Cryptography;
using System.Text;

namespace Trabajo_API_NET.Servicios
{
    public interface IGeneradorID
    {
        string NuevoID(int length = 7); 
    }

    public class GeneradorID : IGeneradorID
    {
        private const string Alfabeto = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string NuevoID(int length = 7)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            var bytes = RandomNumberGenerator.GetBytes(length);
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int idx = bytes[i] % Alfabeto.Length;
                sb.Append(Alfabeto[idx]);
            }
            return sb.ToString();
        }
    }
}
