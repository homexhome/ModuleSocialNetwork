using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data.Context;
using SocialNetwork.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data
{
    public class FriendsRepository : Repository<Friend>
    {
        public FriendsRepository(ApplicationDbContext db) : base(db) {

        }

        public async Task AddFriendAsync(User target, User Friend) {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends == null) {
                var item = new Friend() {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = Friend,
                    CurrentFriendId = Friend.Id,
                };

                await Create(item);
            }
        }

        public async Task<List<User>> GetFriendsByUserAsync(User target) {
            var friends = Set.Include(x => x.CurrentFriend)
                .Where(x => x.UserId == target.Id)
                .Select(x => x.CurrentFriend);

            return await friends.ToListAsync();
        }

        public async Task DeleteFriendAsync(User target, User Friend) {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends != null) {
                await Delete(friends);
            }
        }

    }
}
