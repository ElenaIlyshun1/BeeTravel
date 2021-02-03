using BeeTravel.Data;
using BeeTravel.Entities;
using BeeTravel.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Entities
{
    public class SeederDB
    {
        public static void SeedData(IServiceProvider services,
           IWebHostEnvironment env, IConfiguration config)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<UserManager<DbUser>>();
                var managerRole = scope.ServiceProvider.GetRequiredService<RoleManager<DbRole>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleName = "Admin";
                var result = managerRole.CreateAsync(new DbRole
                {
                    Name = roleName,
                    RoleColor = "#fa4251"
                }).Result;

                roleName = "User";
                result = managerRole.CreateAsync(new DbRole
                {
                    Name = roleName,
                    RoleColor = "#00b5e9"
                }).Result;

                string email = "admin@gmail.com";
                string user_name = "admin";
                string firstlast = "admin";
                string lastname = "administration";
                var user = new DbUser
                {
                    Email = email,
                    UserName = user_name,
                    Firstname = firstlast,
                    Lastname = lastname,
                    PhoneNumber = "+11(111)111-11-11",
                    Image = "admin.png"
                };
                result = manager.CreateAsync(user, "33Ki9x66-3of+s").Result;
                result = manager.AddToRoleAsync(user, "Admin").Result;
            }
        }
        public static void SeedTourData(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(Categories.Select(c => c.Value));
            }

            #region tblTours - Тури
            SeedTours(context, new Tour
            {
                Name = "«Мікс вікенд: Будапешт + Відень»",
                Img = "pic02.jpg",
                ImgLarge = "back_1.jpg",
                Description = "Тур на 4 дні, 2 нічних переїзди. Вечірній виїзд зі Львова і прибуття у Львів у першій половині дня (в залежності від проходження кордону)<br/>" +
                                      "Відвідаємо чудове міста Будапешт та попадемо на дегустацію угорських вин у Єгері(в Угорщині),<br/>" +
                                      "а також побачимо Відень(Австрія).Ці міста не залишуть Вас байдужими,<br/>" +
                                      "а закохають в себе з першого погляду<br/>" +
                                      "У вартість туру включена оглядова екскурсія по м.Будапешт<br/>" +
                                      "Проживання у готелях категорії 3 * на території Угорщини.Сніданки включені у вартість<br/>",
                DepartureTown = "Львів",
                Period = 4,
                IsNightCrossing = true,
                Countries = "Авcтрія, Угорщина",
                Price = 35,
                DepartureDate = "07/02/2020",
                isFavorite = true,
                Category = Categories["Touristic"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Прага - моє улюблене місто»",
                Img = "pic03.jpg",
                ImgLarge = "back_2.jpg",
                DepartureTown = "Львів",
                Description = "Тур на 4 днів з двома нічними переїздами. Денний виїзд з України та денне прибуття в Україну (в залежності від проходження кордону)<br/>" +
                                      "Ночівля у готелі категорії 3* на території Чехії (1 ніч)<br/>" +
                                      "У турі відвідаємо: Польщу (Краків), Чехію (Прагу) та Німеччину (Дрезден)<br/>" +
                                      "У вартості туру оглядова екскурсія по Празі<br/>",
                Period = 4,
                IsNightCrossing = true,
                Countries = "Польща, Німеччина, Чехія",
                Price = 30,
                DepartureDate = "07/02/2020",
                isFavorite = true,
                Category = Categories["Touristic"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Європейська мозаїка: Краків-Прага-Відень-Будапешт»",
                Img = "pic04.jpg",
                ImgLarge = "back_3.jpg",
                DepartureTown = "Львів",
                Description = "Тур на 4 дні, без нічних переїздів. Ранковий виїзд зі Львова і нічне прибуття у Львів (в залежності від проходження кордону)<br/>" +
                                    "Відвідаємо чарівні міста у Польщі (Краків), у Чехії (Прага), в Австрії(Відень), в Угорщині (Будапешт, Єгер, Єгерсалок). У Єгерсалоку можна сповна насолодитись природою і отримати релакс в термальних купальнях.<br/>" +
                                    "У вартість туру включена оглядова екскурсія у Празі - «Вулицями Старого міста»" +
                                    "Проживання у готелях категорії 3* на території Чехії та Угорщини. Сніданки включені у вартість<br/>",
                Period = 4,
                IsNightCrossing = false,
                Countries = "Австрія, Польща, Угорщина, Чехія",
                Price = 115,
                DepartureDate = "14/02/2020",
                isFavorite = true,
                Category = Categories["Touristic"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Чарівність Парижу!»",
                Img = "pic05.jpg",
                ImgLarge = "back_4.jpg",
                Description = "Тур на 6 днів: виїзд у п’ятницю зранку, повернення у середу (в залежності від проходження кордону)<br/>" +
                                    "Відвідаємо Мюнхен, погуляємо вуличками романтичного Парижу (2 дні), познайомимось з фінансовою столицею Німеччини, або як його ще називають мандрівники «ворота в Європу» , місто хмарочосів – Франкфурт на Майні, також, є можливість потрапити на дегустацію вин та гуляшу в Егері<br/>" +
                                    "Тур без нічних переїздів, ночівля у чудових готелях категорії 3* на території Польщі, Німеччини, Франції" +
                                    "У вартість туру включена оглядова екскурсія по історичній частині м. Ніцца, оглядова екскурсія в старій частині міста Марсель та оглядова екскурсія в старій частині міста Генуя, екскурсія по місту Верона<br/>",
                DepartureTown = "Львів",
                Period = 6,
                IsNightCrossing = false,
                Countries = "Франція, Німеччина, Угорщина",
                Price = 218,
                DepartureDate = "28/02/2020",
                isFavorite = true,
                Category = Categories["Touristic"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Погляд в минуле: Рим, Помпеї, Неаполь»",
                Img = "pic06.jpg",
                ImgLarge = "back_5.jpg",
                DepartureTown = "Львів",
                Description = "Тур на 6 днів: виїзд у п’ятницю зранку, повернення у середу (в залежності від проходження кордону)<br/>" +
                                    "Відвідаємо Мюнхен, погуляємо вуличками романтичного Парижу (2 дні), познайомимось з фінансовою столицею Німеччини, або як його ще називають мандрівники «ворота в Європу» , місто хмарочосів – Франкфурт на Майні, також, є можливість потрапити на дегустацію вин та гуляшу в Егері<br/>" +
                                    "Тур без нічних переїздів, ночівля у чудових готелях категорії 3* на території Польщі, Німеччини, Франції" +
                                    "У вартість туру включена оглядова екскурсія по історичній частині м. Ніцца, оглядова екскурсія в старій частині міста Марсель та оглядова екскурсія в старій частині міста Генуя, екскурсія по місту Верона<br/>",
                Period = 6,
                IsNightCrossing = false,
                Countries = "Угорщина, Австрія, Італія, Ватикан",
                Price = 218,
                DepartureDate = "05/03/2020",
                isFavorite = true,
                Category = Categories["Touristic"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Ласкаві хвилі Адріатики»",
                Img = "pic07.jpg",
                ImgLarge = "back_6.jpg",
                DepartureTown = "Львів",
                Description = "Тур на 10 днів, 2 нічних переїзди.",
                Period = 10,
                IsNightCrossing = true,
                Countries = "Хорватія, Угорщина",
                Price = 320,
                DepartureDate = "22/05/2020",
                isFavorite = true,
                Category = Categories["Relax"]
            });

            SeedTours(context, new Tour
            {
                Name = "«Золоті піски Іспанії!»",
                Img = "pic08.jpg",
                ImgLarge = "back_7.jpg",
                Description = "Тур на 13 днів, 2 нічних переїзди. Вечірній виїзд зі Львова і вечірнє прибуття у Львів (в залежності від проходження кордону)<br/>" +
                                    "Завітаємо у неймовірні міста: Венеція, Генуя та Верона (Італія); Ніцца, Монако, Марсель (Франція); іспанський курорт Пінеда-де-Мар; Барселона, Жирона, Фігерес та будемо мати нагоду завітати у тематичний парк атракціонів Порт Авентура (Іспанія)<br/>" +
                                    "6 днів на морі - курорт Пінеда-де-Мар<br/>" +
                                    "У вартість туру включена оглядова екскурсія по історичній частині м. Ніцца, оглядова екскурсія в старій частині міста Марсель та оглядова екскурсія в старій частині міста Генуя, екскурсія по місту Верона<br/>" +
                                    "Проживання у готелях категорії 3* на території Італії, Франції, Іспанії (6 ночей). Сніданки включені у вартість в готелях<br/>",
                DepartureTown = "Львів",
                Period = 13,
                IsNightCrossing = true,
                Countries = "Іспанія, Італія, Франція, Монако",
                Price = 320,
                DepartureDate = "06/05/2020",
                isFavorite = true,
                Category = Categories["Relax"]
            });
            #endregion          
        }
        public static void SeedTours(ApplicationDbContext context, Tour model)
        {
            var tour = context.Tours.SingleOrDefault(t => t.Name == model.Name);
            if (tour == null)
            {
                tour = new Tour
                {
                    Id = model.Id,
                    Name = model.Name,
                    Img = model.Img,
                    ImgLarge = model.ImgLarge,
                    Description = model.Description,
                    DepartureTown = model.DepartureTown,
                    Period = model.Period,
                    IsNightCrossing = model.IsNightCrossing,
                    Countries = model.Countries,
                    Price = model.Price,
                    DepartureDate = model.DepartureDate,
                    isFavorite = model.isFavorite,
                };
                context.Tours.Add(tour);
                context.SaveChanges();
            }
        }

        private static Dictionary<string, Category> category;
        public static Dictionary<string, Category> Categories
        {
            get
            {
                if (category == null)
                {
                    var list = new Category[]
                        {
                            new Category {CategoryName = "Touristic", Description = "For sightseeing and touristic"},
                            new Category {CategoryName = "Relax", Description = "For relax"},
                        };

                    category = new Dictionary<string, Category>();
                    foreach (Category item in list)
                    {
                        category.Add(item.CategoryName, item);
                    }
                }
                return category;
            }
        }
    }
}
