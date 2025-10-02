using GameShopModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace DatabaseMS.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DatabaseController : ControllerBase
    {
        private ApplicationContext db;
        private ILogger<DatabaseController> logger;

        // Конструктор
        public DatabaseController(ApplicationContext db, ILogger<DatabaseController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        /// <summary>
        /// Получение записи по ID из таблицы TABLE_NAME
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         "id": 2,
        ///         "TABLE_NAME": "Users"
        ///     }
        ///     
        /// </remarks>
        /// <param name="id">Уникальный идентификатор</param>
        /// <param name="TABLE_NAME">Название таблицы</param>
        /// <returns>Запись из таблицы базы данных</returns>
        /// <response code="200">Успешное выполнение. Возврат записи.</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Не найдена таблица или запись</response>
        [HttpGet]
        public async Task<IResult> GetOne(int id, string TABLE_NAME)
        {
            string path = "api/Database/GetOne";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            Object obj;
            // Выбор нужной таблицы с получением записи
            switch (TABLE_NAME)
            {
                case "Users":
                    obj = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                case "Games":
                    obj = await db.Games.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                case "DLCs":
                    obj = await db.DLCs.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                case "Transactions":
                    obj = await db.Transactions.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                case "Images":
                    obj = await db.Images.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                case "Genres":
                    obj = await db.Genres.FirstOrDefaultAsync(u => u.Id == id);
                    break;
                default:
                    logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                    // Срабатывает, если таблица не найдена
                    return Results.NotFound(new { message = "Таблицы с таким именем не существует" });
            }

            // Срабатывает, если запись не найдена
            if (obj == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                return Results.NotFound(new { message = "Записи с таким id не существует" });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если запись найдена
            return Results.Json(obj);
        }

        /// <summary>
        /// Получение всех записей из таблицы TABLE_NAME
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         "TABLE_NAME": "Users"
        ///     }
        ///     
        /// </remarks>
        /// <param name="TABLE_NAME">Название таблицы</param>
        /// <returns>Массив записей из таблицы базы данных</returns>
        /// <response code="200">Успешное выполнение. Возврат массива.</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Таблица не найдена</response>
        [HttpGet]
        public async Task<IResult> GetAll(string TABLE_NAME)
        {
            string path = "api/Database/GetAll";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Выбор нужной таблицы с получением записи
            switch (TABLE_NAME)
            {
                case "Users":
                    User[] users = await db.Users.ToArrayAsync();
                    return Results.Json(users);
                case "Games":
                    Game[] games = await db.Games.ToArrayAsync();
                    return Results.Json(games);
                case "DLCs":
                    DLC[] dlcs = await db.DLCs.ToArrayAsync();
                    return Results.Json(dlcs);
                case "Transactions":
                    Transaction[] transactions = await db.Transactions.ToArrayAsync();
                    return Results.Json(transactions);
                case "Images":
                    Image[] images = await db.Images.ToArrayAsync();
                    return Results.Json(images);
                case "Genres":
                    Genre[] genres = await db.Genres.ToArrayAsync();
                    return Results.Json(genres);
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
            // Срабатывает, если таблица не найдена
            return Results.StatusCode(404);
        }

        /// <summary>
        /// Получение ID последней по ID записи транзакции из таблицы Transactions
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET
        ///     {
        ///         
        ///     }
        ///     
        /// </remarks>
        /// <returns>ID записи</returns>
        /// <response code="200">Успешное выполнение. Возврат ID.</response>
        /// <response code="404">В таблице нет записей</response>
        [HttpGet]
        public async Task<int> GetLastTransaction()
        {
            string path = "api/Database/GetLastTransaction";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Упорядочивание по id и взятие последней записи таблицы
            Transaction res = await (from v in db.Transactions orderby v.Id descending select v).FirstOrDefaultAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Вывод id последней записи
            return res.Id;
        }

        [HttpGet]
        public async Task<User> GetUser(int id)
        {
            string path = "api/Database/GetUser";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return user;
        }

        [HttpGet]
        public async Task<Game> GetGame(int id)
        {
            string path = "api/Database/GetGame";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            Game game = await db.Games.FirstOrDefaultAsync(u => u.Id == id);

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return game;
        }

        [HttpGet]
        public async Task<DLC> GetDLC(int id)
        {
            string path = "api/Database/GetDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            DLC dlc = await db.DLCs.FirstOrDefaultAsync(u => u.Id == id);

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return dlc;
        }

        [HttpGet]
        public async Task<Game[]> GetAllGames()
        {
            string path = "api/Database/GetAllGames";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            Game[] res = await db.Games.ToArrayAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        [HttpGet]
        public async Task<DLC[]> GetAllDLCs()
        {
            string path = "api/Database/GetAllDLCs";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            DLC[] res = await db.DLCs.ToArrayAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return res;
        }

        /// <summary>
        /// Проверка наличия пользователя в базе данных
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///     
        ///     GET
        ///     {
        ///         "username": "ImVolkano",
        ///         "password": "1234567890"
        ///     }
        ///     
        /// </remarks>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Успешное выполнение. Пользователь найден</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpGet]
        public async Task<IResult> CheckUser(string username, string password)
        {
            string path = "api/Database/CheckUser";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Поиск пользователя по username и password
            User user = await db.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == PasswordHashinator.HashPassword(password));

            // Срабатывает, если пользователь не найден
            if (user == null)
            {
                logger.LogInformation($"api/Database/CreateUser's finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                return Results.StatusCode(404);
            }

            logger.LogInformation($"api/Database/CreateUser's finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если пользователь найден
            return Results.StatusCode(200);
        }

        /// <summary>
        /// Создание записи пользователя в таблице Users
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST
        ///     {
        ///         "id": 0
        ///         "username": "FareWell",
        ///         "email": "farewl@gmail.com",
        ///         "password": "qwerty",
        ///         "balance": 0,
        ///         "cart_Games": [ ],
        ///         "cart_DLCs": [ ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="user">Поля пользователя</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Успешное выполнение. Пользователь добавлен.</response>
        /// <response code="400">Некорректные данные</response>
        [HttpPost]
        public async Task<IResult> CreateUser(User user)
        {
            string path = "api/Database/CreateUser";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Хэширование пароля
            user.Password = PasswordHashinator.HashPassword(user.Password);

            // Попытка добавления пользователя
            try
            {
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            } catch (Exception ex)
            {
                logger.LogInformation($"api/Database/CreateUser's finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.StatusCode(400);
            }

            logger.LogInformation($"api/Database/CreateUser's finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении пользователя
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateGame(Game game)
        {
            string path = "api/Database/CreateGame";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавления игры
            try
            {
                await db.Games.AddAsync(game);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении игры
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateDLC(DLC dlc)
        {
            string path = "api/Database/CreateDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить DLC
            try
            {
                await db.DLCs.AddAsync(dlc);
                await db.SaveChangesAsync();
            } catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении DLC
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateTransaction(Transaction transaction)
        {
            string path = "api/Database/CreateTransaction";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить транзакцию
            try
            {
                await db.Transactions.AddAsync(transaction);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении транзакции
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateImage(Image img)
        {
            string path = "api/Database/CreateImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить изображение
            try
            {
                await db.Images.AddAsync(img);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении изображения
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateGenre(Genre genre)
        {
            string path = "api/Database/CreateGenre";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить жанр
            try
            {
                await db.Genres.AddAsync(genre);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении изображения
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateTransactionInfo(TransactionInfo tinfo)
        {
            string path = "api/Database/CreateTransactionInfo";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить информацию о транзакции
            try
            {
                await db.TransactionInfos.AddAsync(tinfo);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении ифнормации о транзакции
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateGameToImage(GameToImage gImage)
        {
            string path = "api/Database/CreateGameToImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить связь между игрой и картинкой
            try
            {
                await db.GamesToImages.AddAsync(gImage);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении связи
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateGameToDLC(GameToDLC gDLC)
        {
            string path = "api/Database/CreateGameToDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить связь между игрой и DLC
            try
            {
                await db.GamesToDLCs.AddAsync(gDLC);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении связи
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateGameToGenre(GameToGenre gGenre)
        {
            string path = "api/Database/CreateGameToGenre";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить связь между игрой и жанром
            try
            {
                await db.GamesToGenres.AddAsync(gGenre);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении связи
            return Results.StatusCode(200);
        }

        [HttpPost]
        public async Task<IResult> CreateDLCToImage(DLCToImage dlcImage)
        {
            string path = "api/Database/CreateDLCToImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Попытка добавить связь между DLC и картинкой
            try
            {
                await db.DLCsToImages.AddAsync(dlcImage);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает при успешном добавлении связи
            return Results.StatusCode(200);
        }

        /// <summary>
        /// Изменение данных пользователя в таблице Users
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT
        ///     {
        ///         "id": 2,
        ///         "username": "iVolkano",
        ///         "email": "maks_kasimov@internet.ru",
        ///         "password": "0987654321",
        ///         "balance": 1200,
        ///         "cart_Games": [ 1 ],
        ///         "cart_DLCs": [ ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="new_user">Поля пользователя</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Успешное выполнение. Данные изменены.</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpPut]
        public async Task<IResult> UpdateUser(User new_user)
        {
            string path = "api/Database/UpdateUser";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Извлечение ID
            int old_id = new_user.Id;

            // Поиск пользователя по ID
            User user = await db.Users.FirstOrDefaultAsync(u => u.Id == old_id);

            // Срабатывает, если пользователь не найден
            if (user == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                return Results.NotFound(new { message = "Записи с таким id нет" });
            }

            user.Username = new_user.Username;
            user.Email = new_user.Email;
            user.Password = PasswordHashinator.HashPassword(new_user.Password); // Хэширование пароля
            user.Balance = new_user.Balance;
            user.Cart_Games = new_user.Cart_Games;
            user.Cart_DLCs = new_user.Cart_DLCs;

            // Попытка сохранить изменения
            try
            {
                await db.SaveChangesAsync();
            } catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message});
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если изменения сохранились
            return Results.StatusCode(200);
        }

        [HttpPut]
        public async Task<IResult> UpdateGame(Game new_game)
        {
            string path = "api/Database/UpdateGame";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Взятие ID
            int old_id = new_game.Id;

            // Поиск по ID
            Game game = await db.Games.FirstOrDefaultAsync(u => u.Id == old_id);

            // Срабатывает, если запись не найдена
            if (game == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Записи с таким id нет" });
            }

            game.Name = new_game.Name;
            game.Description = new_game.Description;
            game.Price = new_game.Price;
            game.Quantity = new_game.Quantity;
            game.Sale = new_game.Sale;

            // Попытка сохранить изменения
            try
            {
                await db.SaveChangesAsync();
            } catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Выпало исключение: " + ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если изменения сохранились
            return Results.StatusCode(200);
        }

        [HttpPut]
        public async Task<IResult> UpdateDLC(DLC new_dlc)
        {
            string path = "api/Database/UpdateDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            int old_id = new_dlc.Id;

            DLC dlc = await db.DLCs.FirstOrDefaultAsync(u => u.Id == old_id);

            if (dlc == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Записи с таким id нет" });
            }

            dlc.Name = new_dlc.Name;
            dlc.Description = new_dlc.Description;
            dlc.Price = new_dlc.Price;
            dlc.Sale = new_dlc.Sale;
            dlc.Quantity = new_dlc.Quantity;

            await db.SaveChangesAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }

        [HttpPut]
        public async Task<IResult> UpdateTransaction(Transaction new_transaction)
        {
            string path = "api/Database/UpdateTransaction";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            int old_id = new_transaction.Id;

            Transaction transaction = await db.Transactions.FirstOrDefaultAsync(u => u.Id == old_id);

            if (transaction == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Записи с таким id нет" });
            }

            transaction.User_id = new_transaction.User_id;
            transaction.Date = new_transaction.Date;

            await db.SaveChangesAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }

        [HttpPut]
        public async Task<IResult> UpdateImage(Image new_image)
        {
            string path = "api/Database/UpdateImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            int old_id = new_image.Id;

            Image image = await db.Images.FirstOrDefaultAsync(u => u.Id == old_id);

            if (image == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Записи с таким id нет" });
            }

            image.Image_Name = new_image.Image_Name;

            await db.SaveChangesAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }

        [HttpPut]
        public async Task<IResult> UpdateGenre(Genre new_genre)
        {
            string path = "api/Database/UpdateGenre";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            int old_id = new_genre.Id;

            Genre genre = await db.Genres.FirstOrDefaultAsync(u => u.Id == old_id);

            if (genre == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = "Записи с таким id нет" });
            }

            genre.Name = new_genre.Name;

            await db.SaveChangesAsync();

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            return Results.Ok();
        }

        /// <summary>
        /// Удаление записи пользователя из таблицы Users
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     DELETE
        ///     {
        ///         "id": 3
        ///     }
        ///     
        /// </remarks>
        /// <param name="id">Уникальный идентификатор</param>
        /// <returns>Статус-код</returns>
        /// <response code="200">Успешное выполнение. Запись удалена.</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpDelete]
        public async Task<IResult> DeleteUser(int id)
        {
            string path = "api/Database/DeleteUser";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            User obj = db.Users.FirstOrDefault(u => u.Id == id);

            // Срабатывает, если пользователь не найден
            if(obj == null)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {404}");
                return Results.NotFound();
            }

            // Попытка удалить запись
            try
            {
                db.Users.Remove(obj);
                await db.SaveChangesAsync();
            } catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new {message = ex.Message});
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteGame(int id)
        {
            string path = "api/Database/DeleteGame";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            Game obj = db.Games.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.Games.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteDLC(int id)
        {
            string path = "api/Database/DeleteDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            DLC obj = db.DLCs.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.DLCs.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteGenre(int id)
        {
            string path = "api/Database/DeleteGenre";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            Genre obj = db.Genres.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.Genres.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteImage(int id)
        {
            string path = "api/Database/DeleteImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            Image obj = db.Images.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.Images.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteTransaction(int id)
        {
            string path = "api/Database/DeleteTransaction";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            Transaction obj = db.Transactions.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.Transactions.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteTransactionInfo(int id)
        {
            string path = "api/Database/DeleteTransactionInfo";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            TransactionInfo obj = db.TransactionInfos.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.TransactionInfos.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteGameToDLC(int id)
        {
            string path = "api/Database/GameToDLC";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            GameToDLC obj = db.GamesToDLCs.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.GamesToDLCs.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }
        
        [HttpDelete]
        public async Task<IResult> DeleteGameToImage(int id)
        {
            string path = "api/Database/DeleteGameToImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            GameToImage obj = db.GamesToImages.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.GamesToImages.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteGameToGenre(int id)
        {
            string path = "api/Database/DeleteGameToGenre";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            GameToGenre obj = db.GamesToGenres.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.GamesToGenres.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }

        [HttpDelete]
        public async Task<IResult> DeleteDLCToImage(int id)
        {
            string path = "api/Database/DeleteDLCToImage";
            logger.LogInformation($"{path}'s started at {DateTime.Now.ToLongTimeString()}");

            // Получение объекта для удаления
            DLCToImage obj = db.DLCsToImages.FirstOrDefault(u => u.Id == id);

            // Попытка удалить запись
            try
            {
                db.DLCsToImages.Remove(obj);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {400}");
                return Results.BadRequest(new { message = ex.Message });
            }

            logger.LogInformation($"{path}'s finished at {DateTime.Now.ToLongTimeString()} with status-code {200}");
            // Срабатывает, если удаление завершено успешно
            return Results.StatusCode(200);
        }
    }
}
