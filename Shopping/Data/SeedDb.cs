using Microsoft.EntityFrameworkCore;
using Shopping.Data.Entities;
using Shopping.Enums;
using Shopping.Helpers;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper UserHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = UserHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountriesAsync();
            await CheckProductsAsync();
            await CheckRolesAsync();
            //await CheckUserAsync("1010", "Luis", "Urquidy", "kyhory@yopmail.com", "668 164 0547", "Calle Luna Calle Sol", UserType.Admin);
            //await CheckUserAsync("2020", "Kyhorito", "Urquidy", "kyhorito@yopmail.com", "668 164 0547", "Calle Luna Calle Sol", UserType.User);
            await CheckUserAsync("1010", "Luis", "Urquidy", "kyhory@yopmail.com", "668 164 0547", "Calle Luna Calle Sol", null, UserType.Admin);
            await CheckUserAsync("2020", "kyhorito", "Urquidy", "kyhorito@yopmail.com", "668 164 0547", "Calle Luna Calle Sol", null, UserType.User);
            await CheckUserAsync("3030", "Paola", "Pillado", "paola@yopmail.com", "668 265 3774", "Calle Luna Calle Sol", null, UserType.Admin);
            await CheckUserAsync("4040", "Isabella", "Urquidy", "isabella@yopmail.com", "668 265 3774", "Calle Luna Calle Sol", null, UserType.User);

        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Memoria Usb 32GB Kingston", 100M, 1F, new List<string>() { "Tecnología", "Computación" }, new List<string>() { "USB32kingston.png" });
                await AddProductAsync("Memoria Usb 64GB Adata", 160M, 2F, new List<string>() { "Tecnología", "Computación" }, new List<string>() { "USB64adata.png" });
                await AddProductAsync("Toner 1147 Compatible Kyocera", 600M, 3F, new List<string>() { "Tecnología", "Computación", "Multifuncional" }, new List<string>() { "kyo1147.png", "kyo1147_2.png" });
                await AddProductAsync("Teclado Gamer", 500M, 1F, new List<string>() { "Gamer", "Tecnología", "Computación" }, new List<string>() { "tecladogamer.png" });
                await _context.SaveChangesAsync();
            }

        }

        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string>? images)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string? category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            //foreach (string? image in images)
            //{
            //    Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
            //    prodcut.ProductImages.Add(new ProductImage { ImageId = imageId });
            //}

            _context.Products.Add(prodcut);
        }


        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string image,
            UserType userType)      
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = Guid.Empty;                                                                                                      // se añadio esta linea para evitar conflicto al no tener imagen y no puder subir azure blob
                //if (image != null || image != "")                                                                                             // se añadio esta linea para evitar conflicto al no tener imagen y no puder subir azure blob
                //{                                                                                                                             // se añadio esta linea para evitar conflicto al no tener imagen y no puder subir azure blob
                //    imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users"); // se añadio esta linea para evitar conflicto al no tener imagen y no puder subir azure blob
                //}                                                                                                                             // se añadio esta linea para evitar conflicto al no tener imagen y no puder subir azure blob
                //Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageId = imageId
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }
        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }
        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = new List<City>() {
                                new City() { Name = "Medellín" },
                                new City() { Name = "Itagüí" },
                                new City() { Name = "Envigado" },
                                new City() { Name = "Bello" },
                                new City() { Name = "Sabaneta" },
                                new City() { Name = "La Ceja" },
                                new City() { Name = "La Union" },
                                new City() { Name = "La Estrella" },
                                new City() { Name = "Copacabana" },
                            }
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = new List<City>() {
                                new City() { Name = "Usaquen" },
                                new City() { Name = "Champinero" },
                                new City() { Name = "Santa fe" },
                                new City() { Name = "Usme" },
                                new City() { Name = "Bosa" },
                            }
                        },
                        new State()
                        {
                            Name = "Valle",
                            Cities = new List<City>() {
                                new City() { Name = "Calí" },
                                new City() { Name = "Jumbo" },
                                new City() { Name = "Jamundí" },
                                new City() { Name = "Chipichape" },
                                new City() { Name = "Buenaventura" },
                                new City() { Name = "Cartago" },
                                new City() { Name = "Buga" },
                                new City() { Name = "Palmira" },
                            }
                        },
                        new State()
                        {
                            Name = "Santander",
                            Cities = new List<City>() {
                                new City() { Name = "Bucaramanga" },
                                new City() { Name = "Málaga" },
                                new City() { Name = "Barrancabermeja" },
                                new City() { Name = "Rionegro" },
                                new City() { Name = "Barichara" },
                                new City() { Name = "Zapatoca" },
                            }
                        },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Florida",
                            Cities = new List<City>() {
                                new City() { Name = "Orlando" },
                                new City() { Name = "Miami" },
                                new City() { Name = "Tampa" },
                                new City() { Name = "Fort Lauderdale" },
                                new City() { Name = "Key West" },
                            }
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = new List<City>() {
                                new City() { Name = "Houston" },
                                new City() { Name = "San Antonio" },
                                new City() { Name = "Dallas" },
                                new City() { Name = "Austin" },
                                new City() { Name = "El Paso" },
                            }
                        },
                        new State()
                        {
                            Name = "California",
                            Cities = new List<City>() {
                                new City() { Name = "Los Angeles" },
                                new City() { Name = "San Francisco" },
                                new City() { Name = "San Diego" },
                                new City() { Name = "San Bruno" },
                                new City() { Name = "Sacramento" },
                                new City() { Name = "Fresno" },
                            }
                        },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "Ecuador",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Pichincha",
                            Cities = new List<City>() {
                                new City() { Name = "Quito" },
                            }
                        },
                        new State()
                        {
                            Name = "Esmeraldas",
                            Cities = new List<City>() {
                                new City() { Name = "Esmeraldas" },
                            }
                        },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "México",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Sinaloa",
                            Cities = new List<City>() {
                                new City() { Name = "Los Mochis" },
                                new City() { Name = "El Carrizo" },
                                new City() { Name = "El Fuerte" },
                                new City() { Name = "Ejido 9 de Diciembre" },
                                new City() { Name = "Ejido Benito Juarez" },
                                new City() { Name = "Bachomobampo" },
                                new City() { Name = "Ejido Plan de Ayala" },
                                new City() { Name = "Ejido Plan de San Luis" },
                                new City() { Name = "Topolobampo" },
                                new City() { Name = "1 de Mayo" },
                                new City() { Name = "5 de Mayo" },
                                new City() { Name = "Guasave" },
                            }
                        },
                        new State()
                        {
                            Name = "Sonora",
                            Cities = new List<City>() {
                                new City() { Name = "Navojoa" },
                                new City() { Name = "Hermosillo" },
                                new City() { Name = "Obregon" },
                            }
                        },
                    }
                });
            }
            await _context.SaveChangesAsync();
        }
        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Apple" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Computación" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Multifuncional" });
                _context.Categories.Add(new Category { Name = "Nutrición" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Tecnología" });

                await _context.SaveChangesAsync();
            }
        }
    }
}


