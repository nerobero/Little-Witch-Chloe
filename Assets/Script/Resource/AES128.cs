using System;
using System.Text;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using UnityEngine.InputSystem;

public class AES128
{
    private string key
    {
        get
        {
            MD5 md5 = MD5.Create();

            string salt = "madebydaon";
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(salt));
            return Encoding.UTF8.GetString(result);
        }
    }

    RijndaelManaged rijndealCipher;

    public AES128()
    {
        rijndealCipher = new RijndaelManaged();

        rijndealCipher.Mode = CipherMode.CBC;
        rijndealCipher.Padding = PaddingMode.PKCS7;
        rijndealCipher.KeySize = 128;
        rijndealCipher.BlockSize = 128;
    }

    public string Encrypt(string textToEncrypt)
    {
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[128];

        int len = pwdBytes.Length;
        if(len > keyBytes.Length) len = keyBytes.Length;

        Array.Copy(pwdBytes, keyBytes, len);

        rijndealCipher.Key = keyBytes;
        rijndealCipher.IV = keyBytes;

        ICryptoTransform transform = rijndealCipher.CreateEncryptor();

        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    public string Decrypt(string textToDecrypt)
    {
        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[128];

        int len = pwdBytes.Length;
        if(len > keyBytes.Length) len = keyBytes.Length;

        Array.Copy(pwdBytes, keyBytes, len);

        rijndealCipher.Key = keyBytes;
        rijndealCipher.IV = keyBytes;

        ICryptoTransform transform = rijndealCipher.CreateDecryptor();

        return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length));
    }
}
