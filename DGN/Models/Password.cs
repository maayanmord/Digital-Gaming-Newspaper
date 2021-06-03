using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace DGN.Models
{
    public class Password
    {
        [ForeignKey("User")]
        public int Id { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }

        [Required]
        public User User { get; set; }

        public Password() {
        }
        private Password(int id, byte[] hash, byte[] salt) {
            Initialize(id, hash, salt);
        }

        /// <summary>
        /// <para>This function creates Password using the clear password</para>
        /// Solting with random bytes and Using sha256
        /// </summary>
        /// <param name="PlainTextPassword">The password as plain text</param>
        public Password(int id, string PlainTextPassword, User user) {
            byte[] salt = CreateSalt();
            Initialize(id, CreateHash(PlainTextPassword, salt), salt);
            User = user;
        }

        private void Initialize(int id, byte[] hash, byte[] salt)
        {
            Id = id;
            Hash = hash;
            Salt = salt;
        }

        /// <summary>
        /// This function is check if the password is currect
        /// </summary>
        /// <param name="PlainTextPassword"></param>
        /// <returns>true if currect, false if not</returns>
        public bool Check(string PlainTextPassword)
        {
            byte[] checkHash = CreateHash(PlainTextPassword, Salt);

            if (Hash.Length != checkHash.Length)
            {
                return false;
            }

            for (int i = 0; i < checkHash.Length; i++)
            {
                if (checkHash[i] != Hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// <para>This function is creating 32 byte salt.</para>
        /// Salt is random bytes that are added to the clear password before hash
        /// </summary>
        /// <returns>
        /// The salt
        /// </returns>
        private static byte[] CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// <para>This function creates Password using the clear password and salt</para>
        /// Solting with the given solt and Using sha256
        /// </summary>
        /// <param name="PlainTextPassword">The password as plain text</param>
        /// <param name="salt">The salt to use</param>
        private static byte[] CreateHash(string PlainTextPassword, byte[] Salt)
        {
            // Salting
            byte[] pwdBytes = System.Text.Encoding.ASCII.GetBytes(PlainTextPassword);
            byte[] SaltedPassword = new byte[Salt.Length + pwdBytes.Length];

            for (int i = 0; i < pwdBytes.Length; i++)
            {
                SaltedPassword[i] = pwdBytes[i];
            }

            for (int i = 0; i < Salt.Length; i++)
            {
                SaltedPassword[i + pwdBytes.Length] = Salt[i];
            }

            // Creating the Hash
            HashAlgorithm algorithm = new SHA256Managed();
            return algorithm.ComputeHash(SaltedPassword);
        }
    }
}
