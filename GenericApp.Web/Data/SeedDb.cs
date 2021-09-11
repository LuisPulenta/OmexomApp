using GenericApp.Common.Enums;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApp.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            //await _context.Database.EnsureCreatedAsync();
            //await CheckCountriesAsync();
            //await CheckRolesAsync();
            //await CheckStatesAsync();
            //await CheckUserAsync("17157729", "Luis", "Núñez", "luisalbertonu@gmail.com", "156 814 963", "Espora 2052", UserType.Admin);

        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
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
                    FavoriteTeam = _context.Teams.FirstOrDefault(),
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }


        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new CountryEntity
                {
                    Name = "Argentina",
                    Departments = new List<DepartmentEntity>
                {
                    new DepartmentEntity
                    {
                        Name = "Córdoba",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "Córdoba" },
                            new CityEntity { Name = "Río Cuarto" },
                            new CityEntity { Name = "Villa María" }
                        }
                    },
                    new DepartmentEntity
                    {
                        Name = "Buenos Aires",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "La Plata" },
                            new CityEntity { Name = "Mar del  Plata" },
                            new CityEntity { Name = "Tandil" }
                        }
                    },
                    new DepartmentEntity
                    {
                        Name = "Santa Fe",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "Santa Fe" },
                            new CityEntity { Name = "Rosario" },
                            new CityEntity { Name = "Venado Tuerto" }
                        }
                    }
                },
                    Teams = new List<TeamEntity>
                {
                    new TeamEntity
                    {
                        Name = "Talleres",
                    },
                    new TeamEntity
                    {
                        Name = "Belgrano",
                    },
                    new TeamEntity
                    {
                        Name = "River Plate",
                    },
                    new TeamEntity
                    {
                        Name = "Boca Juniors",
                    },
                    }
                });

                _context.Countries.Add(new CountryEntity
                {
                    Name = "USA",
                    Departments = new List<DepartmentEntity>
                {
                    new DepartmentEntity
                    {
                        Name = "California",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "Los Angeles" },
                            new CityEntity { Name = "San Diego" },
                            new CityEntity { Name = "San Francisco" }
                        }
                    },
                    new DepartmentEntity
                    {
                        Name = "Illinois",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "Chicago" },
                            new CityEntity { Name = "Springfield" }
                        }
                    },
                    new DepartmentEntity
                    {
                        Name = "Florida",
                        Cities = new List<CityEntity>
                        {
                            new CityEntity { Name = "Miami" },
                            new CityEntity{ Name = "Orlando" }
                        }
                    }
                },
                    Teams = new List<TeamEntity>
                {
                    new TeamEntity
                    {
                        Name = "San Antonio Spurs",
                    },
                    new TeamEntity
                    {
                        Name = "Los Angeles Lakers",
                    },
                    new TeamEntity
                    {
                        Name = "Miami Heats",
                    },
                    new TeamEntity
                    {
                        Name = "New York Knicks",
                    },
                    }
                });

            };
            await _context.SaveChangesAsync();
        }

        private async Task CheckStatesAsync()
        {
            if (!_context.States.Any())
            {
                _context.States.Add(new StateEntity
                {
                    Name = "Sin Iniciar",
                });
                _context.States.Add(new StateEntity
                {
                    Name = "Iniciado",
                });
                _context.States.Add(new StateEntity
                {
                    Name = "Pendiente",
                });
                _context.States.Add(new StateEntity
                {
                    Name = "Terminado",
                });
            };
            await _context.SaveChangesAsync();
        }
    }
}