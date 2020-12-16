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
                    Name = roleName
                }).Result;

                roleName = "User";
                result = managerRole.CreateAsync(new DbRole
                {
                    Name = roleName
                }).Result;

                string email = "admin@gmail.com";
                var user = new DbUser
                {
                    Email = email,
                    UserName = email,
                    PhoneNumber = "+11(111)111-11-11"
                };
                result = manager.CreateAsync(user, "33Ki9x66-3of+s").Result;
                result = manager.AddToRoleAsync(user, roleName).Result;
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
                Img = "1.jpg",
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
                Img = "2.jpg",
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
                Img = "3.jpg",
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
                Img = "4.jpg",
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
                Img = "5.jpg",
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
                Img = "6.jpg",
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
                Img = "7.jpg",
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

            SeedTours(context, new Tour
            {
                Name = "«Відпочинок з насолодою: Болгарія і Греція»",
                Img = "8.jpg",
                ImgLarge = "back_8.jpg",
                Description = "Тур на 9 днів, виїзд зі Львова о 08:00, в останній день нічне повернення у Львів (орієнтовно після 00:00, в залежності від проходження кордону)<br/>" +
                                    "На Вас чекає відпочинок на курорті Сонячний Берег в Болгарії та на побережжі чарівного півострова - Кассандра в Греції<br/>" +
                                    "Тур з 1 - м нічним переїздом,<br/>" +
                                    "6 ночей у чудових готелях категорії 3 * в Болгарії,<br/>" +
                                    "Греції та 1 ночівля в транзитному готелі на території Сербії<br/>" +
                                    "Захоплююча подорож з відпочинком на морі в 2 - х країнах і з можливістю відвідати старовинне місто Несебр,<br/>" +
                                    "яке повністю знаходиться під захистом ЮНЕСКО; восьме чудо світу – Метеори; а також відвідати винні підвали в Долині красунь з дегустацією вин і смачного гуляшу<br/>",
                DepartureTown = "Львів",
                Period = 9,
                IsNightCrossing = true,
                Countries = "Болгарія, Греція, Угорщина",
                Price = 355,
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
