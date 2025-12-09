using System.Security.Cryptography;
using System.Text;

namespace Elixir.KeyGeneration;

public class Program
{
    static void Main(string[] args)
    {
        //var emails = new string[] { "seventh@yopmail.com", "janer@yopmail.com", "sangeetha1@yopmail.com", "arun@yopmail.com", "siva@yopmail.com", "buvih@yopmail.com",
        //    "km@yopmail.com","act@yopmail.com","kmm@yopmail.com"
        //};
        //foreach (var email in emails)
        //{
        //    var emailhash = GenerateIntegerHashForString(email);
        //    var salt = GenerateSalt();
        //    var pwd = GeneratePassword();
        //    var pwdhash = PasswordWithSaltHashGeneration(pwd, salt);
        //    Console.WriteLine($"'{email}', '{emailhash}', '{salt}, {pwdhash}' : Password: {pwd}");
        //}

        //string encryptionKey, encryptionIV;
        //GenerateKeyAndIV(out encryptionKey, out encryptionIV);

        //Console.WriteLine($"AES Key: {encryptionKey}");
        //Console.WriteLine($"IV: {encryptionIV}");
    }

    public static void RSAKeyGeneration()
    {
        Console.WriteLine("Generating new RSA keys...");

        RSA rsa = RSA.Create();
        rsa.KeySize = 2048; // Recommended key size

        string newPrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        string newPublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

        Console.WriteLine("Keys Generated Successfully!");
        Console.WriteLine("============================");
        Console.WriteLine($"New Private Key : ");
        Console.WriteLine("===================");
        Console.WriteLine();
        Console.WriteLine(newPrivateKey);
        Console.WriteLine($"New Public Key : ");
        Console.WriteLine("==================");
        Console.WriteLine(newPublicKey);
    }

    public static void SecretKeyGeneration()
    {
        var secretKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        Console.WriteLine($"Generated Secret Key: {secretKey}");
    }

    public static void GenerateKeyAndIV(out string key, out string iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateKey();  // Generates a secure 32-byte key
            aes.GenerateIV();   // Generates a secure 16-byte IV

            key = Convert.ToBase64String(aes.Key);
            iv = Convert.ToBase64String(aes.IV);
        }
    }

    public static string GenerateSalt()
    {
        var randomData = new byte[32];
        RandomNumberGenerator.Fill(randomData);
        //Console.WriteLine($"Generated Salt : {Convert.ToBase64String(randomData)}");
        return Convert.ToBase64String(randomData);
    }

    public static string PasswordWithSaltHashGeneration(string password, string saltData)
    {
        var salt = Convert.FromBase64String(saltData);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[]? hash;
        using (SHA256 sha = SHA256.Create())
        {
            sha.TransformBlock(salt, 0, salt.Length, salt, 0);
            sha.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
            hash = sha.Hash;
        }
        //Console.WriteLine($"Generated Password With Salt Hash: {Convert.ToBase64String(hash)}");
        return Convert.ToBase64String(hash);
    }

    public static int GenerateIntegerHashForString(string data, bool IgnoreCase = true)
    {
        if (IgnoreCase)
            data = data.ToLowerInvariant(); // Use `ToLowerInvariant()` for better performance & consistency

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            // Combine values using XOR from different positions for better bit distribution
            return BitConverter.ToInt32(hashedBytes, 0) ^
                   BitConverter.ToInt32(hashedBytes, 8) ^
                   BitConverter.ToInt32(hashedBytes, 24);
        }

    }

    public static string GeneratePassword()
    {
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&*";

        Random random = new Random();

        // Generate 2 random numbers
        char number1 = numbers[random.Next(numbers.Length)];
        char number2 = numbers[random.Next(numbers.Length)];

        // Generate 1 random special character
        char specialChar = specialChars[random.Next(specialChars.Length)];

        // Combine "TMI" + [random number] + [special character] + [random number]
        string password = $"TMI{number1}{specialChar}{number2}";

        return password;

    }

}
