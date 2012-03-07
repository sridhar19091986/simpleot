using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SimpleOT.Domain;
using System.Data;

namespace SimpleOT.Data
{
    public class PlayerRepository : Repository
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PlayerRepository(IConnectionFactory connectionFactory)
            : base(connectionFactory)
        {
        }

        public Player Load(string name)
        {
            var query = new StringBuilder();

            query.Append(" SELECT players.id AS id, players.name AS name, accounts.name AS accname,");
            query.Append(" account_id, sex, vocation, experience, level, maglevel, health,");
            query.Append(" groups.name AS groupname, groups.flags AS groupflags, groups.access AS access,");
            query.Append(" groups.maxviplist AS maxviplist, groups.maxdepotitems AS maxdepotitems, groups.violation AS violation,");
            query.Append(" healthmax, mana, manamax, manaspent, soul, direction, lookbody,");
            query.Append(" lookfeet, lookhead, looklegs, looktype, lookaddons, posx, posy,");
            query.Append(" posz, cap, lastlogin, lastlogout, lastip, conditions, skull_time,");
            query.Append(" skull_type, loss_experience, loss_mana, loss_skills,");
            query.Append(" loss_items, loss_containers, rank_id, guildnick, town_id, balance, stamina");
            query.Append(" FROM players");
            query.Append(" LEFT JOIN accounts ON account_id = accounts.id");
            query.Append(" LEFT JOIN groups ON groups.id = players.group_id");
            query.Append(" WHERE players.name = @playerName");

            Player player = null;

            try
            {
                using (var connection = ConnectionFactory.Get())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query.ToString();
                        command.CommandType = CommandType.Text;

                        var playerName = command.CreateParameter();
                        playerName.ParameterName = "@playerName";
                        playerName.Value = name;
                        playerName.DbType = DbType.String;
                        command.Parameters.Add(playerName);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                player = new Player();

                                player.Id = (int)reader["id"];
                                player.Name = (string)reader["name"];

                                player.Account = new Account();
                                player.Account.Id = (int)reader["account_id"];
                                player.Account.Name = (string)reader["accname"];
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorException("Can't load player.", exception);
            }

            return player;
        }
    }
}
