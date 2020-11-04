using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resinBot.Core.Database
{
    public class Data
    {
        /// <summary>
        /// Create a database spot for user, assuming they dont already exist. Will then return that user if created
        /// </summary>
        /// <param name="discId"></param>
        /// <returns></returns>
        public static DiscordUser CreateDiscordUser(ulong discId)
        {
            // Open database (or create if doesn't exist)
            using (var db = new SqliteDbContext())
            {
                // gather user
                var user = db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).FirstOrDefault();
                if (user != null)
                    return user;

                // user does not exist, create them
                var discUser = new DiscordUser()
                {
                    DiscId = discId
                };

                // user was created, now add to database
                db.Add(discUser);
                db.SaveChanges();

                return discUser;
            }
        }

        /// <summary>
        /// does the user exist in the database?
        /// </summary>
        /// <param name="discId"></param>
        /// <returns></returns>
        public static bool UserExists(ulong discId)
        {
            // Open database (or create if doesn't exist)
            using (var db = new SqliteDbContext())
            {
                // gather user
                return db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).Count() > 0;
            }
        }

        /// <summary>
        /// Gets the users last set resin value
        /// </summary>
        /// <param name="discId"></param>
        /// <returns></returns>
        public static int GetLastResin(ulong discId)
        {
            using (var db = new SqliteDbContext())
            {
                var user = db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).FirstOrDefault();
                if (user == null)
                    user = CreateDiscordUser(discId);
                return user.LastResin;
            }
        }
        /// <summary>
        /// Sets the users last resin value
        /// </summary>
        /// <param name="discId"></param>
        /// <param name="resin"></param>
        public static void SetLastResin(ulong discId, int resin )
        {
            // create if null
            CreateDiscordUser(discId);

            using (var db = new SqliteDbContext())
            {
                var user = db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).FirstOrDefault();
                if (user == null)
                    user = CreateDiscordUser(discId);

                // set values
                user.LastResin = resin;
                db.Update(user);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the users last set resin ticks
        /// </summary>
        /// <param name="discId"></param>
        /// <returns></returns>
        public static long GetResinTicks(ulong discId)
        {
            using (var db = new SqliteDbContext())
            {
                var user = db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).FirstOrDefault();
                if (user == null)
                    user = CreateDiscordUser(discId);
                return user.ResinSetTicks;
            }
        }
        /// <summary>
        /// Sets the users last resin datetime ticks
        /// </summary>
        /// <param name="discId"></param>
        /// <param name="resin"></param>
        public static void SetResinTicks(ulong discId, long ticks)
        {
            // create if null
            CreateDiscordUser(discId);

            using (var db = new SqliteDbContext())
            {
                var user = db.DiscordUsers.AsQueryable().Where(x => x.DiscId.Equals(discId)).FirstOrDefault();
                if (user == null)
                    user = CreateDiscordUser(discId);

                // set values
                user.ResinSetTicks = ticks;
                db.Update(user);
                db.SaveChanges();
            }
        }
    }
}
