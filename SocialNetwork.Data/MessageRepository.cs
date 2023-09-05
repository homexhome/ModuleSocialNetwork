using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data.Context;
using SocialNetwork.Models.Db;

namespace SocialNetwork.Data
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(ApplicationDbContext db)
        : base(db)
        {

        }

        public async Task<List<Message>> GetMessages(User sender, User recipient) {
            // Assuming Set is an Entity Framework DbSet<Message> or similar
            var query = Set
                .Include(x => x.Recipient)
                .Include(x => x.Sender)
                .Where(x =>
                    (x.SenderId == sender.Id && x.RecipientId == recipient.Id) ||
                    (x.SenderId == recipient.Id && x.RecipientId == sender.Id))
                .OrderBy(x => x.Id);

            return await query.ToListAsync();
        }
    }
}
