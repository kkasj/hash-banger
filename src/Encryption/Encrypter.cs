using System.Security.Cryptography;
using System.Text;

namespace design_patterns.Encryption;

public class EncrypterFactory
{
    public static IEncrypter CreateEncrypter(EncryptionType encryptionType)
    {
        switch (encryptionType)
        {
            case EncryptionType.SHA1:
                return new SHA1Encrypter();
            default:
                throw new ArgumentException("Invalid encryption type");
        }
    }
}   

public interface IEncrypter
{
    string Encrypt(string data);
}

public class SHA1Encrypter : IEncrypter
{
    public string Encrypt(string data)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
