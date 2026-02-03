using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Models;
using System.Security.Principal;

namespace HondaSensorChecker.Data
{
    public static class DatabaseInitializer
    {
        public static void SeedIfMissing(DataContext db, bool dbExists)
        {
            if (dbExists)
                return;

            var userName = GetLoggedUserName();
            var zfId = (userName ?? string.Empty).ToUpperInvariant();

            db.Operators.Add(new Operator
            {
                Re = userName,
                ZfId = zfId,
                Name = userName,
                Admin = true
            });

            db.Products.AddRange(new[]
            {
                new Product { ProductId = 1, Prefix = "C2TG", StartPartNumber = "A013F520", EndPartNumber = "ELMOD00660" },
                new Product { ProductId = 2, Prefix = "C2TT", StartPartNumber = "A013F507", EndPartNumber = "ELMOD00670" },
                new Product { ProductId = 3, Prefix = "C2PK", StartPartNumber = "A021T127", EndPartNumber = "ELMOD00690" },
                new Product { ProductId = 4, Prefix = "C2PK", StartPartNumber = "A027U445", EndPartNumber = "ELMOD00720" },
                new Product { ProductId = 5, Prefix = "M0S5", StartPartNumber = "A010P456", EndPartNumber = "ELSEN00090" },
                new Product { ProductId = 6, Prefix = "G0UN", StartPartNumber = "A013E460", EndPartNumber = "ELSEN00100" },
                new Product { ProductId = 7, Prefix = "L0YU", StartPartNumber = "A010P457", EndPartNumber = "ELSEN00110" },
                new Product { ProductId = 8, Prefix = "M0S8", StartPartNumber = "A011Y164", EndPartNumber = "ELSEN00120" },
                new Product { ProductId = 9, Prefix = "G0UL", StartPartNumber = "A010P458", EndPartNumber = "ELSEN00130" },
                new Product { ProductId = 10, Prefix = "L0X5", StartPartNumber = "A024V584", EndPartNumber = "ELSEN00140" },
                new Product { ProductId = 11, Prefix = "M0SW", StartPartNumber = "A026Y354", EndPartNumber = "ELSEN00150" },
                new Product { ProductId = 12, Prefix = "G0UV", StartPartNumber = "A024V580", EndPartNumber = "ELSEN00160" }
            });

            db.SaveChanges();
        }

        private static string GetLoggedUserName()
        {
            try
            {
                var identityName = WindowsIdentity.GetCurrent()?.Name;
                return NormalizeUserName(identityName ?? Environment.UserName);
            }
            catch
            {
                return NormalizeUserName(Environment.UserName);
            }
        }

        private static string NormalizeUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return string.Empty;

            var withoutDomain = userName.Contains('\\')
                ? userName.Split('\\', 2)[1]
                : userName;

            return withoutDomain.Contains('@')
                ? withoutDomain.Split('@', 2)[0]
                : withoutDomain;
        }
    }
}
