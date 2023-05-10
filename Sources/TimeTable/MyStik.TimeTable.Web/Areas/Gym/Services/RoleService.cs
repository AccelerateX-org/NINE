using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace MyStik.TimeTable.Web.Areas.Gym.Services
{

    public class RoleService
    {
        private GymDbContext _context;

        public RoleService(GymDbContext context)
        {
            _context = context;
        }

        public Author GetAuthor(ApplicationUser user)
        {
            var author = _context.Authors.SingleOrDefault(x => x.UserId.Equals(user.Id));

            if (author != null) return author;

            author = new Author
            {
                UserId = user.Id
            };

            _context.Authors.Add(author);
            _context.SaveChanges();

            return author;
        }

        public Player GetPlayer(ApplicationUser user)
        {
            var player = _context.Players.SingleOrDefault(x => x.UserId.Equals(user.Id));

            if (player != null) return player;

            player = new Player()
            {
                UserId = user.Id
            };

            _context.Players.Add(player);
            _context.SaveChanges();

            return player;
        }

    }
}