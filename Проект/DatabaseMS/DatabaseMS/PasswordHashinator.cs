using System.Security.Cryptography;
using System.Text;

namespace DatabaseMS
{
    public class PasswordHashinator
    {
        public static string HashPassword(string password)
        {
            MD5 md5 = MD5.Create();

            // Хэширование пароля с мощью MD5
            byte[] b = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(b);

            // Построение строки для вывода хэшированной версии пароля
            StringBuilder sb = new StringBuilder();
            foreach (var a in hash)
            {
                sb.Append(a.ToString("X2"));
            }

            // Вывод хэшированной версии пароля в виде строки
            string res = Convert.ToString(sb);
            return res;
        }
    }
}
