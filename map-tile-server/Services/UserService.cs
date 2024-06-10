using map_tile_server.Models.Configurations;
using map_tile_server.Models.Details;
using map_tile_server.Models.Entities;
using MongoDB.Driver;
using Serilog;

namespace map_tile_server.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Otp> _otpCollection;

        public UserService(IDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>(settings.UserCollectionName);
            _otpCollection = database.GetCollection<Otp>(settings.OtpCollectionName);
        }
        public List<UserDetail> Gets(int page, int pageSize, string? key)
        {
            List<User> users;
            if (key != null && key.Trim().Length > 0)
            {
                users = _usersCollection.Find(u => u.FirstName.ToLower().StartsWith(key.ToLower()) || u.LastName.ToLower().StartsWith(key.ToLower()) || u.Username.ToLower().StartsWith(key.ToLower())).ToList();
            }
            else
            {
                users = _usersCollection.Find(FilterDefinition<User>.Empty).ToList();
            }
            int totalUser = users.Count;
            int totalPages = (int)Math.Ceiling((decimal)totalUser / pageSize);

            List<UserDetail> userList = users.Skip((page - 1) * pageSize).Take(pageSize).Select(u => new UserDetail(u)).ToList();

            return userList;
        }

        public int GetCount(string? key)
        {
            List<User> users;
            if (key != null && key.Trim().Length > 0)
            {
                users = _usersCollection.Find(u => u.FirstName.ToLower().StartsWith(key.ToLower()) || u.LastName.ToLower().StartsWith(key.ToLower()) || u.Username.ToLower().StartsWith(key.ToLower())).ToList();
            }
            else
            {
                users = _usersCollection.Find(FilterDefinition<User>.Empty).ToList();
            }
            return users.Count;
        }

        public User? GetById(string id)
        {
            var user = _usersCollection.Find(u => u.Id == id).FirstOrDefault();
            return user;
        }
        public User? GetByEmail(string email)
        {
            var user = _usersCollection.Find(u => u.Email == email).FirstOrDefault();
            return user;
        }

        public UserDetail? GetByEmailAndPassword(LoginDetail detail)
        {
            var user = _usersCollection.Find(u => u.Email == detail.Email && u.Password == detail.Password).FirstOrDefault();
            return user != null ? new UserDetail(user) : null;
        }
        public UserDetail? GetByUsername(string username)
        {
            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            return user != null ? new UserDetail(user) : null;
        }
        public UserDetail Create(User user)
        {
            _usersCollection.InsertOne(user);
            return new UserDetail(user);
        }
        public void Update(string id, User user)
        {
            _usersCollection.ReplaceOne(u => u.Id == id, user);
        }
        public void Delete(string id)
        {
            _usersCollection.DeleteOne(u => u.Id == id);
        }

        public Otp CreateOtp(string email)
        {
            var otp = new Otp { Email = email, Code = GenerateRandomNumberString() };
            _otpCollection.InsertOne(otp);
            return otp;
        }

        public void DeleteOtp(string email, string code)
        {
            var otp = _otpCollection.Find(o => o.Email == email && o.Code == code).FirstOrDefault();
            if (otp != null)
                _otpCollection.DeleteOne(o => o.Id == otp.Id);
        }

        public bool ValidateOtp(string email, string code)
        {
            var otp = _otpCollection.Find(o => o.Email == email && o.Code == code).FirstOrDefault();
            return otp != null;
        }

        public string GenerateRandomNumberString()
        {
            Random random = new Random();
            string randomString = random.Next(100000, 1000000).ToString("D6");
            return randomString;
        }
    }
}
