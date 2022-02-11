using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorSignInSystem.Server.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

/// <summary>
/// VSIS business logic
/// </summary>
namespace VisitorSignInSystem.Server.BLL
{
    public class ComplexProcedures
    {
        // enable locale
        private CultureInfo enUS = new CultureInfo("en-US");

        // TODO: use DI for dbcontext
        private readonly vsisdataContext _context;

        public ComplexProcedures(vsisdataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GetAllCounters
        /// </summary>
        /// <returns></returns>
        public async Task<List<Counter>> GetAllCounters()
        {
            List<Counter> counters = await _context.Counters
                .Select(p => new Counter()
                {
                    Host = p.Host,
                    CounterNumber = p.CounterNumber,
                    Description = p.Description,
                    DisplayDescription = p.DisplayDescription,
                    Location = p.Location,
                    Floor = p.Floor,
                    IsHandicap = p.IsHandicap,
                    IsAvailable = p.IsAvailable,
                    Category = p.Category,
                    Icon = p.Icon,
                    Created = p.Created
                }).ToListAsync();

            return counters;
        }


        /// <summary>
        /// GetTransferReasons
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<Transfer>> GetTransferReasons(int location)
        {
            List<Transfer> xfer = new();
            try
            {
                xfer = await _context.Transfers
                    .Where(p => p.Location == location)
                    .ToListAsync<Transfer>();
            }
            catch (Exception) { }
            return xfer;
        }

        /// <summary>
        /// GetAvailableAgents
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAvailableAgents()
        {
            List<string> agents = await _context.Agents
                .Where(p => p.StatusName == "AVAILABLE")
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        /// <summary>
        /// GetAvailableAgentsByCategory
        /// </summary>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<int> GetAvailableAgentsByCategory(ulong category, int location)
        {
            var param1 = new MySqlConnector.MySqlParameter("@v_cat", category);
            var param2 = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<Agent> agents = await _context.Agents
                .FromSqlRaw($"CALL getAvailableAgentsByCategory(@v_cat, @v_location);",
                new object[] { param1, param2 })
                .ToListAsync();

            return agents.Count;
        }

        /// <summary>
        /// Category descriptions for Agent
        /// </summary>
        /// <returns></returns>
        public async Task<List<AgentCategory>> GetCategoryDescriptionByAgent()
        {
            var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "getCategoriesByAgent";

            _context.Database.OpenConnection();

            List<AgentCategory> cats = new List<AgentCategory>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        AgentCategory cat = new AgentCategory();
                        cat.AuthName = reader["auth_name"].ToString();
                        cat.Categories = int.Parse(reader["categories"].ToString());
                        cat.Description = reader["description"].ToString();
                        cats.Add(cat);
                    }
                }
            }
            return cats;
        }

        /// <summary>
        /// GetUnAvailableAgents
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetUnAvailableAgents()
        {
            List<string> agents = await _context.Agents
                .Where(p => p.StatusName == "UNAVAILABLE")
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        /// <summary>
        /// GetAllAgents
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllAgents()
        {
            List<string> agents = await _context.Agents
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        /// <summary>
        /// Gets connected agents
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns>Agent</returns>
        public async Task<List<Agent>> GetConnectedAgents(string Ids)
        {
            return await _context.Agents
                .Where<Agent>(p => Ids.Contains(p.AuthName)).ToListAsync();
        }

        /// <summary>
        /// GetAgentNames
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAgentNames()
        {
            List<string> agents = await _context.Agents
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        /// <summary>
        /// GetMaxWaitTimeByCategory
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<WaitTimeNotify> GetMaxWaitTimeByCategory(ulong category)
        {
            return await _context.WaitTimeNotifies
                .Where(p => p.Category == category)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// GetOtherAgentsInCall
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<List<Agent>> GetOtherAgentsInCall(string groupName)
        {
            return await _context.Agents
                .Where<Agent>(p => p.AuthName != groupName && p.VisitorId != 0).ToListAsync();
        }

        /// <summary>
        /// Return Agent data from VisitorId
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns>Agent</returns>
        public async Task<Agent> GetAgentByVisitorId(int visitorId)
        {
            return await _context.Agents
                .FirstOrDefaultAsync<Agent>(p => p.VisitorId == visitorId);
        }

        /// <summary>
        /// Return the agent auth name when agent is at counter
        /// ex. groupName = pao-pc103
        /// groupName is counter field in table agents
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<string> GetAgentFromCounter(string groupName)
        {
            return await _context.Agents
               .Where(p => p.Counter == groupName)
               .Select(p => p.AuthName).FirstOrDefaultAsync();
        }

        /// <summary>
        /// GetCounters
        /// </summary>
        /// <returns></returns>
        public async Task<List<Counter>> GetCounters()
        {
            List<Counter> counters = await _context.Counters
                .Where(p => p.IsAvailable == true)
                .Select(p => new Counter()
                {
                    Host = p.Host,
                    CounterNumber = p.CounterNumber,
                    Description = p.Description,
                    Location = p.Location,
                    Floor = p.Floor,
                    IsHandicap = p.IsHandicap,
                    IsAvailable = p.IsAvailable,
                    Icon = p.Icon,
                    Created = p.Created
                }).ToListAsync();

            return counters;
        }

        /// <summary>
        /// GetCountersByCategory
        /// </summary>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<Counter>> GetCountersByCategory(ulong category, int location)
        {
            var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
            var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<Counter> counters = await _context.Counters
                .FromSqlRaw("CALL getCountersByCategory(@v_cat, @v_location);",
                new object[] { cat, loc })
                .ToListAsync();

            return counters;
        }

        /// <summary>
        /// This is a new visitor added from kiosk.
        /// Update the visitor department from the
        /// visitor category id
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns>bool</returns>
        public async Task<bool> SaveNewVisitor(Visitor visitor)
        {
            try
            {
                visitor.VisitDepartmentId = await LookupDepartmentId(visitor.VisitCategoryId);

                _context.Visitors.Add(visitor);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// LookupDepartmentId
        /// </summary>
        /// <param name="visitCategoryId"></param>
        /// <returns></returns>
        public async Task<sbyte> LookupDepartmentId(ulong visitCategoryId)
        {
            return await _context.Categories
                .Where(p => p.Id == visitCategoryId)
                .Select(p => p.DepartmentId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Add Agent metrics for visit to table
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task SaveAgentMetrics(string groupName, int visitorId)
        {
            try
            {
                Visitor v = await GetVisitorByVisitorId(visitorId);
                if (v != null)
                {
                    string auth_agent = groupName;

                    // get agent who served visitor

                    Agent agent = await GetAgentByVisitorId(visitorId);
                    if (agent != null)
                        auth_agent = agent.AuthName;

                    VisitorMetric m = new();
                    m.Agent = auth_agent;
                    m.Kiosk = v.Kiosk;
                    m.AssignedCounter = v.AssignedCounter;
                    m.Created = v.CalledTime.Value;
                    m.IsHandicap = v.IsHandicap;
                    m.Location = v.Location;
                    m.VisitCategoryId = v.VisitCategoryId;
                    m.CallDuration = DateTime.Now;

                    _context.VisitorMetrics.Add(m);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// GetCounterName
        /// </summary>
        /// <param name="counterHost"></param>
        /// <returns></returns>
        public async Task<string> GetCounterName(string counterHost)
        {
            Counter counter = await _context.Counters
                .Where(p => p.Host == counterHost)
                .Select(p => new Counter()
                {
                    Description = p.Description
                }).FirstOrDefaultAsync();

            return counter.Description;
        }

        /// <summary>
        /// GetCategory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetCategory(ulong id)
        {
            var query = from p in _context.Categories
                        where p.Id == id
                        select p.Description;

            return await query.SingleAsync();
        }

        /// <summary>
        /// QueueCount
        /// </summary>
        /// <returns></returns>
        public async Task<int> QueueCount()
        {
            var query = from p in _context.Visitors
                        where p.StatusName == "WAITING"
                        select p.Id;

            return await query.CountAsync();
        }

        /// <summary>
        /// Return queue count by agent name
        /// </summary>
        /// <param name="agent_name"></param>
        /// <returns></returns>
        public async Task<int> QueueCountByAgent(string agent_name)
        {
            var query = from p in _context.Visitors
                        where p.StatusName == "WAITING"
                        select p.Id;

            return await query.CountAsync();
        }

        /// <summary>
        /// GetAllVisitorsTaken
        /// </summary>
        /// <returns></returns>
        public async Task<List<Visitor>> GetAllVisitorsTaken()
        {
            return await _context.Visitors
                .Where(p => p.StatusName == "TAKEN").ToListAsync();
        }

        /// <summary>
        /// GetAgentMetrics
        /// </summary>
        /// <param name="agentName"></param>
        /// <returns></returns>
        public async Task<AgentMetric> GetAgentMetrics(string agentName)
        {
            return await _context.AgentMetrics
                .Where(p => p.AuthName == agentName).FirstOrDefaultAsync();
        }

        /// <summary>
        /// AvailableHandicapCountersCount
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<int> AvailableHandicapCountersCount(int location)
        {
            var query = from p in _context.Counters
                        where p.Location == location && p.IsHandicap == true && p.IsAvailable == true
                        select p.CounterNumber;

            return await query.CountAsync();
        }

        /// <summary>
        /// GetAvailableCountersCount
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<int> GetAvailableCountersCount(string groupName, int location)
        {
            var query = from p in _context.Counters
                        where p.Location == location && p.IsAvailable == true
                        select p.CounterNumber;

            return await query.CountAsync();
        }

        /// <summary>
        /// GetQueueCountByAgent
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<int> GetQueueCountByAgent(string groupName, int location)
        {
            string sql = "";

            // gets the authname when agent is at counter
            string authname = await GetAgentFromCounter(groupName);

            // ok to be null here
            if (authname != "")
            {
                // default sql
                sql = "getVisitorsCountByAgent(@v_auth_name, @v_location, @v_department);";

                Counter counter = await _context.Counters
                .FirstOrDefaultAsync(p => p.Host == groupName);

                if (counter != null)
                {
                    if (authname != null)
                        groupName = authname;

                    // 2021-10-22, gwb, do not exclude non-handicap visitors in queue count
                    //if (counter.IsHandicap)
                    //    sql = "getHandicapVisitorsByAgent(@v_auth_name, @v_location, @v_department);";
                }

                VsisUser user = await UserContext(groupName);
                if (user != null)
                {
                    var param1 = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
                    var param2 = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);
                    var param3 = new MySqlConnector.MySqlParameter("@v_department", user.Department);

                    List<Visitor> vis = await _context.Visitors
                        .FromSqlRaw($"CALL {sql};",
                        new object[] { param1, param2, param3 })
                        .ToListAsync();

                    return vis.Count;
                }
            }
            return 0;
        }

        /// <summary>
        ///  Public service can take any visitor 
        ///  when not a transfer, and when transfer only when in public
        ///  service dept. All others can take visitors only when in category
        ///  regardless of transfer.        
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public async Task<Visitor> GetNextWaitingVisitor(string groupName, int location, string groupType)
        {
            try
            {
                Agent agent = null;
                VsisUser user = null;

                var param1 = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
                var param2 = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);
                var param3 = new MySqlConnector.MySqlParameter("@v_department", "");

                object[] parameters = null;

                // default groupType to Agent
                string sql = "getNextInLineByAgent(@v_auth_name, @v_location, @v_department);";

                switch (groupType)
                {
                    case "Counter":

                        sql = "getNextInLineByCategory(@v_cat, @v_location, @v_department);";

                        agent = await GetCounterFromGroupName(groupName);
                        if (agent != null)
                        {
                            user = await UserContext(agent.AuthName);
                            Counter counter = await GetCounterContext(groupName);

                            param1 = new MySqlConnector.MySqlParameter("@v_cat", counter.Category);
                            param3 = new MySqlConnector.MySqlParameter("@v_department", user.Department);

                            parameters = null;
                            parameters = new object[] { param1, param2, param3 };
                        }
                        break;

                    case "Agent":

                        agent = await AgentContext(groupName);
                        if (agent != null)
                        {
                            user = await UserContext(agent.AuthName);
                            param3 = new MySqlConnector.MySqlParameter("@v_department", user.Department);

                            parameters = null;
                            parameters = new object[] { param1, param2, param3 };
                        }

                        break;
                }

                List<Visitor> vis = await _context.Visitors
                    .FromSqlRaw($"CALL {sql};", parameters)
                    .ToListAsync();

                if (vis.Count > 0)
                {
                    Visitor visitor = new();

                    visitor = vis[0];

                    if (visitor != null)
                    {
                        // take visitor out of queue
                        visitor.StatusName = "TAKEN";
                        await _context.SaveChangesAsync();
                        return visitor;
                    }
                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// UpdateAgentVisitorId
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitor_id"></param>
        /// <returns></returns>
        public async Task UpdateAgentVisitorId(string groupName, int visitor_id)
        {
            // Get a real agent user name to save visitor Id
            // This is helps identify data in agent metrics
            // groupName could be a counter name or an agent name
            // when agent is at a counter, the counter will be assigned to the
            // agent. when agent is at desk, the groupName will be agent auth_name
            Agent agent = await GetCounterFromGroupName(groupName);
            if (agent != null)
                groupName = agent.AuthName;

            // do save
            agent = await AgentContext(groupName);
            if (agent != null)
            {
                agent.VisitorId = visitor_id;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// GetCategories
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<Category>> GetCategories(int location)
        {
            List<Category> cats = new();
            try
            {
                // Order by description with "Other" last

                cats = await _context.Categories
                    .Where(p => p.Location == location)
                    .OrderBy(p => (p.Description.ToUpper() == "OTHER") ? 1 : 0)
                    .ThenBy(p => p.Description)
                    .ToListAsync<Category>();
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return cats;
        }

        /// <summary>
        /// Get categories for all locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Category>> GetAnyCategories()
        {
            List<Category> cats = new();
            try
            {
                // Order by description with "Other" last

                cats = await _context.Categories
                    .OrderBy(p => (p.Description.ToUpper() == "OTHER") ? 1 : 0)
                    .ThenBy(p => p.Description)
                    .ToListAsync<Category>();
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return cats;
        }


        public async Task<List<IconInventory>> GetIconsList()
        {
            List<IconInventory> icons = new();
            try
            {
                icons = await _context.IconInventories
                    .OrderBy(p => p.Icon)
                    .ToListAsync<IconInventory>();
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return icons;
        }

        /// <summary>
        /// GetDisplaysAtLocation
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<GroupDevice>> GetDisplaysAtLocation(int location)
        {
            List<GroupDevice> devices = new();
            try
            {
                devices = await _context.GroupDevices
                    .Where(p => p.Enabled && p.Location == location && p.Kind == "Display")
                    .ToListAsync<GroupDevice>();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
            return devices;
        }

        public async Task<List<GroupDevice>> GroupDevices()
        {
            List<GroupDevice> devices = new();
            try
            {
                devices = await _context.GroupDevices
                    .ToListAsync<GroupDevice>();
            }
            catch (Exception)
            {
            }
            return devices;
        }


        /// <summary>
        /// GetAllLocations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetAllLocations()
        {
            List<Location> locations = new();
            try
            {
                locations = await _context.Locations
                    .Where(p => p.Open == true)
                    .ToListAsync<Location>();
            }
            catch (Exception) { }
            return locations;
        }

        /// <summary>
        /// AgentContext
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<Agent> AgentContext(string groupName)
        {
            return await _context.Agents
            .FirstOrDefaultAsync<Agent>(p => p.AuthName == groupName);
        }

        /// <summary>
        /// GetAgentCounter
        /// </summary>
        /// <param name="agent_name"></param>
        /// <returns></returns>
        public async Task<string> GetAgentCounter(string agent_name)
        {
            return await _context.Agents
                .Where(p => p.AuthName == agent_name)
                .Select(p => p.Counter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// IsCounter
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool?> IsCounter(string groupName)
        {
            try
            {
                var c = await (from v in _context.Counters
                               where v.Host == groupName
                               select v).ToListAsync();

                return c.Count > 0;

            }
            catch (Exception)
            {
                // Debug.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// IsDepartmentAvailable
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task<bool> IsDepartmentAvailable(sbyte department)
        {
            return (await (from v in _context.VsisUsers
                           join a in _context.Agents on v.AuthName equals a.AuthName
                           where v.Department == department && a.StatusName == "AVAILABLE"
                           select v).CountAsync()) > 0;
        }

        /// <summary>
        /// AgentUserNames
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task RecordUserAppVersion(string groupName, string appVersion)
        {
            VsisUser user = await _context.VsisUsers
                .FirstOrDefaultAsync<VsisUser>(p => p.AuthName == groupName);
            if (user != null)
            {
                bool tf = await IsRoleManager(groupName);
                if (tf)
                {
                    user.ManagerAppVersion = appVersion;
                }
                else
                {
                    user.AgentAppVersion = appVersion;
                }
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// AgentUserNames
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<List<VsisUser>> AgentUserNames(string groupName)
        {
            return await (from v in _context.VsisUsers
                          join c in _context.Agents on v.AuthName equals c.AuthName
                          select v).ToListAsync();
        }

        /// <summary>
        /// GetAgentsByCategory
        /// </summary>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<VsisUser>> GetAgentsByCategory(ulong category, int location)
        {
            var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
            var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<VsisUser> vis = await _context.VsisUsers
                .FromSqlRaw("CALL getAgentsByCategory(@v_cat, @v_location);",
                new object[] { cat, loc })
                .ToListAsync();

            return vis;
        }

        /// <summary>
        /// addAgentMetrics will compute the visitor metrics and 
        /// insert the data into agent_metrics table for retrieval
        /// on the agent stats panel.
        /// The inserted row is returned but not implemented.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool> AddAgentStats(string groupName)
        {
            try
            {
                var auth_name = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);

                List<AgentMetric> a = await _context.AgentMetrics
                    .FromSqlRaw("CALL addAgentMetrics(@v_auth_name);",
                    new object[] { auth_name }).ToListAsync();
            }
            catch (Exception) { }
            return true;
        }

        /// <summary>
        /// addCategoryMetrics will compute the category metrics and 
        /// insert the data into category_metrics table for retrieval
        /// on the manager stats panel.
        /// The inserted row is returned but not implemented.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<bool> AddCategoryStats(ulong category)
        {
            try
            {
                var cat = new MySqlConnector.MySqlParameter("@v_category_id", category);

                List<CategoryMetric> a = await _context.CategoryMetrics
                    .FromSqlRaw("CALL addCategoryMetrics(@v_category_id);",
                    new object[] { cat }).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// GetAgentByCategory
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<string> GetAgentByCategory(string groupName, ulong category, int location)
        {
            try
            {
                var authname = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
                var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
                var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

                List<Agent> agent = await _context.Agents
                    .FromSqlRaw("CALL getAgentByCategory(@v_auth_name, @v_cat, @v_location);",
                    new object[] { authname, cat, loc })
                    .ToListAsync();

                if (agent.Count > 0)
                    return agent[0].AuthName;
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return "";
        }

        /// <summary>
        /// GetCounterFromGroupName
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<Agent> GetCounterFromGroupName(string groupName)
        {
            return await _context.Agents
                .FirstOrDefaultAsync(p => p.Counter == groupName);
        }

        /// <summary>
        /// UserContext
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<VsisUser> UserContext(string groupName)
        {
            return await _context.VsisUsers
            .FirstOrDefaultAsync<VsisUser>(p => p.AuthName == groupName);
        }

        public async Task<List<VsisUserDetail>> Users()
        {
            return await (from u in _context.VsisUsers
                          join d in _context.Departments on u.Department equals d.Id
                          join a in _context.Agents on u.AuthName equals a.AuthName
                          select new VsisUserDetail
                          {
                              AuthName = u.AuthName,
                              FullName = u.FullName,
                              LastName = u.LastName,
                              Department = u.Department,
                              DepartmentName = d.DepartmentName,
                              Categories = a.Categories,
                              Role = u.Role,
                              Location = u.Location,
                              Active = u.Active,
                              Created = u.Created
                          }).ToListAsync();
        }

        /// <summary>
        /// Return list of departments with custom properties
        /// </summary>
        /// <returns></returns>
        public async Task<List<DepartmentDetail>> DepartmentDetailList()
        {
            try
            {
                List<Department> d = await _context.Departments.ToListAsync();
                List<DepartmentDetail> u = new List<DepartmentDetail>();

                foreach (Department item in d)
                {
                    u.Add(new DepartmentDetail()
                    {
                        Department = item.Id,
                        DepartmentName = item.DepartmentName
                    });
                }
                return u;
            }
            catch (Exception) { }
            return null;
        }

        public async Task<List<Department>> DepartmentList()
        {
            try
            {
                return await _context.Departments.ToListAsync();
            }
            catch (Exception) { }
            return null;
        }

        public async Task<List<Transfer>> TransferTypesForManager()
        {
            try
            {
                return await _context.Transfers.ToListAsync();
            }
            catch (Exception) { }
            return null;
        }

        public async Task<List<WaitTimeNotify>> WaitTimes()
        {
            return await _context.WaitTimeNotifies
                .ToListAsync();
        }

        /// <summary>
        /// GetCounterContext
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task<Counter> GetCounterContext(string counter_host)
        {
            Counter counter = await _context.Counters
            .FirstOrDefaultAsync<Counter>(p => p.Host == counter_host);

            Agent agent = await _context.Agents
                .FirstOrDefaultAsync<Agent>(p => p.Counter == counter_host);

            if (agent != null)
            {
                bool is_available = agent.StatusName == "AVAILABLE";
                counter.IsAvailable = is_available;
                await _context.SaveChangesAsync();
            }

            return counter;
        }


        /// <summary>
        /// IsValidCounter
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task<bool> IsValidCounter(string counter_host)
        {
            var counter = await _context.Counters
            .FirstOrDefaultAsync(p => p.Host == counter_host);

            return counter != null;
        }

        /// <summary>
        /// Check if the connected user is a manager
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool> IsRoleManager(string groupName)
        {
            try
            {
                string[] roles = { "Manager" };

                VsisUser user = await UserContext(groupName);
                return user != null && roles.Contains(user.Role);
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Get grouped visitor visit counts
        /// </summary>
        /// <returns></returns>
        public async Task<List<VisitorMetricCounts>> GetVisitorMetricsByCategory()
        {
            try
            {
                return await (from v in _context.VisitorMetrics
                              join c in _context.Categories on v.VisitCategoryId equals c.Id
                              group v by c.Description into g
                              select new VisitorMetricCounts
                              {
                                  CategoryKey = g.Key,
                                  CategoryCount = g.Count()
                              }).ToListAsync();
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Get grouped agent visitor counts
        /// </summary>
        /// <returns></returns>
        //public async Task<List<AgentMetricCounts>> GetAgentsMetricCountsByAgent()
        //{
        //    try
        //    {
        //        return await (from v in _context.AgentMetrics
        //                      group v by v.AuthName into g
        //                      select new AgentMetricCounts
        //                      {
        //                          AgentKey = g.Key,
        //                          AgentCount = g.Count()
        //                      }).ToListAsync();
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return null;
        //}

        /// <summary>
        /// Get all agents by role type
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        //public async Task<List<AgentProfile>> GetAgentStatusList(Dictionary<string, string> Ids)
        public async Task<List<AgentProfile>> GetAgentStatusList(List<KeyValuePair<string, string>> Ids)
        {
            try
            {
                // only for defined roles
                string[] roles = { "Manager", "Agent" };

                string authnames = "";

                foreach (var item in Ids.ToList())
                {
                    bool? iscounter = await IsCounter(item.Value);
                    if (iscounter == true)
                    {
                        string a = await GetAgentFromCounter(item.Value);
                        if (a != null)
                            authnames += a;
                    }
                    else
                    {
                        authnames += item;
                    }

                    if (authnames.Length > 0)
                        authnames += ",";

                    //authnames = string.Join(",", Ids.Select(x => x.Value).ToArray());
                }

                if (authnames.Length > 0)
                {
                    List<AgentProfile> agents = await (from a in _context.Agents
                                                       join u in _context.VsisUsers on a.AuthName equals u.AuthName
                                                       join d in _context.Departments on u.Department equals d.Id
                                                       join v in _context.Visitors on a.VisitorId equals v.Id into visitorsGroup
                                                       from vg in visitorsGroup.DefaultIfEmpty()
                                                       join c in _context.Counters on a.Counter equals c.Host into counterGroup
                                                       from cg in counterGroup.DefaultIfEmpty()
                                                       join m in _context.AgentMetrics on a.AuthName equals m.AuthName into metricsGroup
                                                       from mg in metricsGroup.DefaultIfEmpty()

                                                       where authnames.Contains(a.AuthName) && roles.Contains(u.Role)
                                                       orderby d.OrderBy

                                                       select new AgentProfile
                                                       {
                                                           AuthName = a.AuthName,
                                                           Categories = a.Categories,
                                                           StatusName = a.StatusName,
                                                           VisitorName = $"{vg.FirstName} {vg.LastName}",
                                                           Counter = cg.Description,
                                                           FullName = u.FullName,
                                                           LastName = u.LastName,
                                                           Role = u.Role,
                                                           Active = u.Active,
                                                           Location = u.Location,
                                                           VisitorId = a.VisitorId,
                                                           DepartmentName = d.DepartmentName,
                                                           VisitorsToday = (mg == null ? 0 : mg.VisitorsToday),
                                                           VisitorsWtd = (mg == null ? 0 : mg.VisitorsWtd),
                                                           VisitorsMtd = (mg == null ? 0 : mg.VisitorsMtd),
                                                           VisitorsYtd = (mg == null ? 0 : mg.VisitorsYtd),
                                                           CallTimeToday = (mg == null ? 0 : mg.CallTimeToday),
                                                           CallTimeWtd = (mg == null ? 0 : mg.CallTimeWtd),
                                                           CallTimeMtd = (mg == null ? 0 : mg.CallTimeMtd),
                                                           CallTimeYtd = (mg == null ? 0 : mg.CallTimeYtd)

                                                       }).ToListAsync();

                    // add category description to each agent
                    List<AgentCategory> cats = await GetCategoryDescriptionByAgent();

                    foreach (var cat in cats)
                    {
                        foreach (var item in agents.Where(p => p.AuthName == cat.AuthName))
                        {
                            item.CategoriesDescription = cat.Description;
                        }
                    }
                    return agents;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// GetAgentStatusMgr
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        public async Task<AgentProfile> GetAgentStatusMgr(string authName)
        {
            try
            {
                string[] roles = { "Manager", "Agent" };

                AgentProfile agent = await (from a in _context.Agents
                                            join u in _context.VsisUsers on a.AuthName equals u.AuthName
                                            join d in _context.Departments on u.Department equals d.Id
                                            join v in _context.Visitors on a.VisitorId equals v.Id into visitorsGroup
                                            from vg in visitorsGroup.DefaultIfEmpty()
                                            join c in _context.Counters on a.Counter equals c.Host into counterGroup
                                            from cg in counterGroup.DefaultIfEmpty()
                                            join m in _context.AgentMetrics on a.AuthName equals m.AuthName into metricsGroup
                                            from mg in metricsGroup.DefaultIfEmpty()

                                            where authName.Contains(a.AuthName) && roles.Contains(u.Role)
                                            orderby d.OrderBy

                                            select new AgentProfile
                                            {
                                                AuthName = a.AuthName,
                                                Categories = a.Categories,
                                                StatusName = a.StatusName,
                                                VisitorName = $"{vg.FirstName} {vg.LastName}",
                                                Counter = cg.Description,
                                                FullName = u.FullName,
                                                LastName = u.LastName,
                                                Role = u.Role,
                                                Active = u.Active,
                                                Location = u.Location,
                                                DepartmentName = d.DepartmentName,
                                                VisitorId = a.VisitorId,
                                                VisitorsToday = (mg == null ? 0 : mg.VisitorsToday),
                                                VisitorsWtd = (mg == null ? 0 : mg.VisitorsWtd),
                                                VisitorsMtd = (mg == null ? 0 : mg.VisitorsMtd),
                                                VisitorsYtd = (mg == null ? 0 : mg.VisitorsYtd),
                                                CallTimeToday = (mg == null ? 0 : mg.CallTimeToday),
                                                CallTimeWtd = (mg == null ? 0 : mg.CallTimeWtd),
                                                CallTimeMtd = (mg == null ? 0 : mg.CallTimeMtd),
                                                CallTimeYtd = (mg == null ? 0 : mg.CallTimeYtd)

                                            }).FirstOrDefaultAsync();

                if (agent != null)
                {
                    // add category description to each agent
                    List<AgentCategory> cats = await GetCategoryDescriptionByAgent();

                    agent.CategoriesDescription = cats.FirstOrDefault(x => x.AuthName == agent.AuthName).Description;

                    return agent;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Get all counter data
        /// </summary>
        /// <returns></returns>
        public async Task<List<CounterDetail>> GetAllCountersStatus()
        {
            try
            {
                List<CounterDetail> counter = null;

                var query = await (from c in _context.Counters

                                   join a in _context.Agents on c.Host equals a.Counter into agentGroup
                                   from ag in agentGroup.DefaultIfEmpty()

                                   join v in _context.Visitors on ag.Counter equals v.AssignedCounter into visitorsGroup
                                   from vg in visitorsGroup.DefaultIfEmpty()

                                   join u in _context.VsisUsers on ag.AuthName equals u.AuthName into usersGroup
                                   from ug in usersGroup.DefaultIfEmpty()

                                   select new
                                   {
                                       Host = c.Host,
                                       CounterNumber = c.CounterNumber,
                                       Description = c.Description,
                                       CounterStatus = c.IsAvailable ? "Open" : "Closed",
                                       //AgentStatus = ag.StatusName,
                                       AgentFullName = ug.FullName,
                                       VisitorId = ag.VisitorId
                                       //VisitorStatus = vg.StatusName,
                                       //VisitorFullName = vg.FirstName != null ? $"{vg.FirstName} {vg.LastName}" : ""

                                   }).ToListAsync();

                if (query != null)
                {
                    counter = new List<CounterDetail>();

                    foreach (var p in query)
                    {
                        counter.Add(new CounterDetail
                        {
                            Host = p.Host,
                            CounterNumber = p.CounterNumber,
                            Description = p.Description,
                            CounterStatus = p.CounterStatus,
                            //AgentStatus = p.AgentStatus,
                            AgentFullName = p.AgentFullName,
                            VisitorId = p.VisitorId,
                            //VisitorStatus = p.VisitorStatus,
                            //VisitorFullName = p.VisitorFullName
                        });
                    }
                }
                return counter;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public IQueryable<VisitorDetail> GetVisitorDetails(int visitorId)
        //{
        //    var query =
        //        from v in _context.Visitors
        //        where v.Id == visitorId
        //        join d in _context.Categories on v.VisitCategoryId equals d.Id
        //        join c in _context.Counters on v.AssignedCounter equals c.Host into counterGroup
        //        from cg in counterGroup.DefaultIfEmpty()
        //        select new VisitorDetail
        //        {
        //            FirstName = v.FirstName,
        //            LastName = v.LastName,
        //            CounterDescription = cg.Description,
        //            CategoryDescription = d.Description
        //        };

        //    return query;
        //}

        public IQueryable<CounterDetail> GetCountersStatus(string host)
        {
            try
            {
                return from c in _context.Counters
                       where c.Host == host
                       join a in _context.Agents on c.Host equals a.Counter into agentGroup
                       from ag in agentGroup.DefaultIfEmpty()

                       join v in _context.Visitors on ag.Counter equals v.AssignedCounter into visitorsGroup
                       from vg in visitorsGroup.DefaultIfEmpty()

                       join u in _context.VsisUsers on ag.AuthName equals u.AuthName into usersGroup
                       from ug in usersGroup.DefaultIfEmpty()

                       select new CounterDetail
                       {
                           Host = c.Host,
                           CounterNumber = c.CounterNumber,
                           Description = c.Description,
                           CounterStatus = c.IsAvailable ? "Open" : "Closed",
                           //AgentStatus = ag.StatusName,
                           AgentFullName = ug.FullName,
                           VisitorId = ag.VisitorId
                           //VisitorStatus = vg.StatusName,
                           //VisitorFullName = vg.FirstName != null ? $"{vg.FirstName} {vg.LastName}" : ""

                       };
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public async Task<CounterDetail> GetCountersStatus(string host)
        //{
        //    try
        //    {
        //        CounterDetail counter = null;

        //        var query = await from c in _context.Counters
        //                           where c.Host == host
        //                           join a in _context.Agents on c.Host equals a.Counter into agentGroup
        //                           from ag in agentGroup.DefaultIfEmpty()

        //                           join v in _context.Visitors on ag.Counter equals v.AssignedCounter into visitorsGroup
        //                           from vg in visitorsGroup.DefaultIfEmpty()

        //                           join u in _context.VsisUsers on ag.AuthName equals u.AuthName into usersGroup
        //                           from ug in usersGroup.DefaultIfEmpty()

        //                           select new
        //                           {
        //                               CounterNumber = c.CounterNumber,
        //                               Description = c.Description,
        //                               CounterStatus = c.IsAvailable ? "Open" : "Closed",
        //                               //AgentStatus = ag.StatusName,
        //                               AgentFullName = ug.FullName,
        //                               VisitorId = ag.VisitorId
        //                               //VisitorStatus = vg.StatusName,
        //                               //VisitorFullName = vg.FirstName != null ? $"{vg.FirstName} {vg.LastName}" : ""

        //                           };

        //        //if (query != null)
        //        //{
        //        //    counter = new List<CounterDetail>();

        //        //    foreach (var p in query)
        //        //    {
        //        //        counter.Add(new CounterDetail
        //        //        {
        //        //            CounterNumber = p.CounterNumber,
        //        //            Description = p.Description,
        //        //            CounterStatus = p.CounterStatus,
        //        //            //AgentStatus = p.AgentStatus,
        //        //            AgentFullName = p.AgentFullName,
        //        //            VisitorId = p.VisitorId,
        //        //            //VisitorStatus = p.VisitorStatus,
        //        //            //VisitorFullName = p.VisitorFullName
        //        //        });
        //        //    }
        //        //}
        //        //return counter;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// GetAgentStatus
        /// </summary>
        /// <param name="agent_name"></param>
        /// <returns></returns>
        public async Task<string> GetAgentStatus(string agent_name)
        {
            Agent agent = await AgentContext(agent_name);
            if (agent != null)
                return agent.StatusName;

            return "";
        }

        /// <summary>
        /// GetCounterStatus
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task<bool> GetCounterStatus(string counter_host)
        {
            Counter counter = await _context.Counters
                .FirstOrDefaultAsync(p => p.Host == counter_host);

            if (counter != null)
            {
                return counter.IsAvailable;
            }
            return false;
        }

        /// <summary>
        /// AssignAgentToCounter
        /// </summary>
        /// <param name="agentName"></param>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task AssignAgentToCounter(string agentName, string counter_host)
        {
            // get counter context for update
            Counter counter = await _context.Counters
                .FirstOrDefaultAsync(p => p.Host == counter_host);

            if (counter != null)
                await SetAgentCounterHost(agentName, counter.Host);

        }

        /// <summary>
        /// SetCounterAssigned
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task SetCounterAssigned(string counter_host)
        {
            Counter counter = await _context.Counters
                .FirstOrDefaultAsync(p => p.Host == counter_host);

            if (counter != null)
            {
                counter.IsAvailable = false;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// SetVisitorAssignedCounter
        /// </summary>
        /// <param name="visitor_id"></param>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task<Visitor> SetVisitorAssignedCounter(int visitor_id, string counter_host)
        {
            Visitor visitor = null;
            visitor = await _context.Visitors
                .OrderBy(o => o.Created)
                .FirstOrDefaultAsync(p => p.Id == visitor_id);

            if (visitor != null)
            {
                visitor.AssignedCounter = counter_host;
                visitor.StatusName = "ASSIGNED";
                await _context.SaveChangesAsync();
            }
            return visitor;
        }

        /// <summary>
        /// SetAgentStatus
        /// </summary>
        /// <param name="agent_name"></param>
        /// <param name="statusName"></param>
        /// <returns></returns>
        public async Task SetAgentStatus(string agent_name, string statusName)
        {
            var agent = await _context.Agents
                              .Where(p => p.AuthName == agent_name)
                              .FirstOrDefaultAsync();

            if (agent != null)
            {
                agent.StatusName = statusName;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// SetAgentCounter
        /// </summary>
        /// <param name="agent_name"></param>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task SetAgentCounterHost(string agent_name, string counter_host)
        {
            var agent = await _context.Agents
                              .Where(p => p.AuthName == agent_name)
                              .FirstOrDefaultAsync();

            if (agent != null)
            {
                agent.Counter = counter_host;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// ClearAgentCounter
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task ClearAgentCounter(string counter_host)
        {
            var agent = await _context.Agents
                              .Where(p => p.Counter == counter_host)
                              .FirstOrDefaultAsync();
            if (agent != null)
            {
                agent.Counter = "";
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cancel the taken visitor by putting visitor
        /// back in queue
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<bool> CancelCallVisitor(int visitorId)
        {
            /*
             * Get visitor assigned to agent
             * Reset visitor to waiting
             * Check if counter was assigned and make it available
             * */
            bool success = false;

            Visitor visitor = await GetVisitorByVisitorId(visitorId);

            //Visitor visitor = await _context.Visitors
            //    .FirstOrDefaultAsync(p => p.Id == visitorId);

            if (visitor != null)
            {
                string temp_AssignedCounter = "";

                if (visitor.AssignedCounter != null)
                    temp_AssignedCounter = visitor.AssignedCounter;

                visitor.StatusName = "WAITING";
                visitor.AssignedCounter = null;

                // agent will be skipped if null
                Agent agent = await GetAgentByVisitorId(visitorId);

                if (agent != null)
                    agent.VisitorId = 0;

                if (temp_AssignedCounter != "")
                {
                    var counter = await _context.Counters
                        .Where(p => p.Host == temp_AssignedCounter)
                        .FirstOrDefaultAsync<Counter>();

                    if (counter != null)
                        counter.IsAvailable = true;
                }
                await _context.SaveChangesAsync();
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Clear the visitor Id on agent and set
        /// Agent status to available.
        /// Includes saving Agent stats for visit.
        /// Do this when the visit is over or
        /// when the visitor is transferred.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns>bool</returns>
        public async Task<bool> CloseAgentVisitor(int visitorId)
        {
            try
            {
                // agent will be skipped if null
                Agent agent = await GetAgentByVisitorId(visitorId);
                if (agent != null)
                {
                    if (agent.VisitorId != 0)
                    {
                        // do this before clearing agent
                        await SaveAgentMetrics(agent.AuthName, visitorId);

                        agent.VisitorId = 0;
                        agent.StatusName = "AVAILABLE";
                        await _context.SaveChangesAsync();

                        return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// ClearVisitorAssignedCounter
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<bool> ClearVisitorAssignedCounter(Visitor visitor)
        {
            try
            {
                if (visitor != null)
                {
                    visitor.AssignedCounter = null;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Set present counter available when visitor is transferred
        /// </summary>
        /// <param name="assignedCounter"></param>
        /// <returns></returns>
        public async Task<bool> SetCounterAvailable(string assignedCounter)
        {
            try
            {
                if (assignedCounter != null && assignedCounter.Length > 0)
                {
                    Counter counter = await _context.Counters
                        .Where(p => p.Host == assignedCounter)
                        .FirstOrDefaultAsync();

                    if (counter != null)
                    {
                        counter.IsAvailable = true;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        public async Task<string> AddUser(VsisUserDetail user)
        {
            try
            {
                VsisUser v = new VsisUser();

                v.Active = user.Active;
                v.AuthName = user.AuthName;
                v.Department = user.Department;
                v.FullName = user.FullName;
                v.LastName = user.LastName;
                v.Location = user.Location;
                v.Role = user.Role;


                Agent a = new Agent();
                a.AuthName = user.AuthName;
                a.Categories = user.Categories;
                a.StatusName = "UNAVAILABLE";

                _context.VsisUsers.Add(v);
                _context.Agents.Add(a);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Delete VSIS User
        /// </summary>
        /// <param name="authname"></param>
        /// <returns></returns>
        public async Task<string> DeleteUser(string authname)
        {
            try
            {
                // agent is foreign key, delete first

                Agent agent = await _context.Agents
                    .FirstOrDefaultAsync<Agent>(p => p.AuthName == authname);

                if (agent != null)
                {
                    _context.Agents.Remove(agent);
                    await _context.SaveChangesAsync();

                    VsisUser user = await _context.VsisUsers
                        .FirstOrDefaultAsync<VsisUser>(p => p.AuthName == authname);

                    if (user != null)
                    {
                        _context.VsisUsers.Remove(user);
                        await _context.SaveChangesAsync();
                        return "success";
                    }
                }
                else
                {
                    return "Agent is null";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Update user account
        /// Changes to AuthName not supported,
        /// User should be deleted and re-added
        /// </summary>
        /// <param name="oldAuthname"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> UpdateUser(VsisUserDetail user)
        {
            try
            {
                // check if account already exists
                VsisUser v = await _context.VsisUsers
                    .FirstOrDefaultAsync<VsisUser>(p => p.AuthName == user.AuthName);

                if (v != null)
                {
                    v.Active = user.Active;
                    v.Department = user.Department;
                    v.FullName = user.FullName;
                    v.LastName = user.LastName;
                    v.Location = user.Location;
                    v.Role = user.Role;

                    Agent agent = await _context.Agents
                        .FirstOrDefaultAsync<Agent>(p => p.AuthName == v.AuthName);

                    if (agent != null)
                        agent.Categories = user.Categories;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "Account was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wait"></param>
        /// <returns></returns>
        public async Task<string> AddWaitTimeNotify(WaitTimeNotify wait)
        {
            try
            {
                //WaitTimeNotify w = new WaitTimeNotify();

                //w.Mail = wait.Mail;
                //w.Category = wait.Category;
                //w.MaxWaitTimeMinutes = wait.MaxWaitTimeMinutes;

                _context.WaitTimeNotifies.Add(wait);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        public async Task<string> DeleteWaitTimeNotify(ulong waitcategory)
        {
            try
            {
                WaitTimeNotify wait = await _context.WaitTimeNotifies
                    .FirstOrDefaultAsync<WaitTimeNotify>(p => p.Category == waitcategory);

                if (wait != null)
                {
                    _context.WaitTimeNotifies.Remove(wait);
                    await _context.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


        /// <summary>
        /// Change Wait Time Notifications
        /// Email is not unique
        /// </summary>
        /// <param name="oldMail"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public async Task<string> UpdateWaitTimeNotify(WaitTimeNotify wait)
        {
            try
            {
                WaitTimeNotify w = await _context.WaitTimeNotifies
                    .FirstOrDefaultAsync<WaitTimeNotify>(p => p.Category == wait.Category);

                if (w != null)
                {
                    w.Mail = wait.Mail;
                    w.MaxWaitTimeMinutes = wait.MaxWaitTimeMinutes;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "existing account";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> AddDepartment(Department dept)
        {
            try
            {
                _context.Departments.Add(dept);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> UpdateDepartment(Department dept)
        {
            try
            {
                Department c = await _context.Departments
                    .FirstOrDefaultAsync<Department>(p => p.Id == dept.Id);

                if (c != null)
                {
                    c.DepartmentName = dept.DepartmentName;
                    c.Id = dept.Id;
                    c.OrderBy = dept.OrderBy;
                    c.Symbol = dept.Symbol;
                    c.SymbolType = dept.SymbolType;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "The department was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Delete department with first checking
        /// if department is used in another table.
        /// Note: manager app will look for "Cannot"
        /// to format dialog message. Do not change it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> DeleteDepartment(int id)
        {
            try
            {
                // check if department is being used in transfers, users, etc.
                var query = await (from deps in _context.Departments
                                   join v in _context.VsisUsers on deps.Id equals v.Department into dGroup
                                   from vg in dGroup.DefaultIfEmpty()
                                   join t in _context.Transfers on deps.Id equals t.Department into tGroup
                                   from tg in tGroup.DefaultIfEmpty()
                                   where deps.Id == id
                                   select new { VDept = (sbyte?)vg.Department, TDept = (sbyte?)tg.Department }).ToListAsync();

                foreach (var v in query)
                {
                    if (v.VDept != null)
                        return "Cannot delete department while it is assigned in Users table.";

                    if (v.TDept != null)
                        return "Cannot delete department while it is assigned in Transfers table.";
                }

                Department d = await _context.Departments
                    .FirstOrDefaultAsync<Department>(p => p.Id == id);

                if (d != null)
                {
                    _context.Departments.Remove(d);
                    await _context.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


        public async Task<string> AddTransferType(Transfer transfer)
        {
            try
            {
                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> UpdateTransferType(Transfer transfer)
        {
            try
            {
                Transfer t = await _context.Transfers
                    .FirstOrDefaultAsync<Transfer>(p => p.Id == transfer.Id);

                if (t != null)
                {
                    t.Active = transfer.Active;
                    t.Department = transfer.Department;
                    t.Description = transfer.Description;
                    t.Icon = transfer.Icon;
                    t.Location = transfer.Location;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "A transfer record was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> DeleteTransferType(ulong id)
        {
            try
            {
                Transfer t = await _context.Transfers
                    .FirstOrDefaultAsync<Transfer>(p => p.Id == id);

                if (t != null)
                {
                    _context.Transfers.Remove(t);
                    await _context.SaveChangesAsync();
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public async Task<string> UpdateCategory(Category category)
        {
            try
            {
                Category c = await _context.Categories
                    .FirstOrDefaultAsync<Category>(p => p.Id == category.Id);

                if (c != null)
                {
                    c.Active = category.Active;
                    c.Description = category.Description;
                    c.DepartmentId = category.DepartmentId;
                    c.Icon = category.Icon;
                    c.Location = category.Location;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "The category was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteCategory(ulong id)
        {
            try
            {
                Category c = await _context.Categories
                    .FirstOrDefaultAsync<Category>(p => p.Id == id);

                if (c != null)
                {
                    _context.Categories.Remove(c);
                    await _context.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


        public async Task<string> AddCounter(Counter counter)
        {
            try
            {
                _context.Counters.Add(counter);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> UpdateCounter(Counter counter)
        {
            try
            {
                // check if account already exists
                Counter c = await _context.Counters
                    .FirstOrDefaultAsync<Counter>(p => p.Host == counter.Host);

                if (c != null)
                {
                    c.CounterNumber = counter.CounterNumber;
                    c.Description = counter.Description;
                    c.DisplayDescription = counter.DisplayDescription;
                    c.Location = counter.Location;
                    c.Floor = counter.Floor;
                    c.IsHandicap = counter.IsHandicap;
                    c.IsAvailable = counter.IsAvailable;
                    c.Category = counter.Category;
                    c.Icon = counter.Icon;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "Counter was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteCounter(string host)
        {
            try
            {
                Counter c = await _context.Counters
                    .FirstOrDefaultAsync<Counter>(p => p.Host == host);

                if (c != null)
                {
                    _context.Counters.Remove(c);
                    await _context.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }


        public async Task<string> AddGroupDevice(GroupDevice device)
        {
            try
            {
                _context.GroupDevices.Add(device);

                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> UpdateGroupDevice(GroupDevice device)
        {
            try
            {
                // check if account already exists
                GroupDevice c = await _context.GroupDevices
                    .FirstOrDefaultAsync<GroupDevice>(p => p.Id == device.Id);

                if (c != null)
                {
                    c.Kind = device.Kind;
                    c.Name = device.Name;
                    c.Description = device.Description;
                    c.Location = device.Location;
                    c.CanReceive = device.CanReceive;
                    c.CanSend = device.CanSend;
                    c.Enabled = device.Enabled;

                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "Device was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // 1/7/2022, gwb, manager change visitor reason and include department id
        public async Task<string> UpdateVisitorCategoryId(int visitorId, ulong visitCategoryId)
        {
            try
            {
                Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId);

                if (visitor != null)
                {
                    visitor.VisitDepartmentId = await LookupDepartmentId(visitCategoryId);
                    visitor.VisitCategoryId = visitCategoryId;
                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "Visitor was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteGroupDevice(int id)
        {
            try
            {
                GroupDevice c = await _context.GroupDevices
                    .FirstOrDefaultAsync<GroupDevice>(p => p.Id == id);

                if (c != null)
                {
                    _context.GroupDevices.Remove(c);
                    await _context.SaveChangesAsync();

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }






        /// <summary>
        /// Flag visitor transferred
        /// The flag will be used to validate
        /// the department.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<bool> FlagTransferVisitor(int visitorId, bool isTransfer)
        {
            try
            {
                Visitor visitor = await GetVisitorByVisitorId(visitorId);

                if (visitor != null)
                {
                    if (visitor.IsTransfer != isTransfer)
                    {
                        visitor.IsTransfer = isTransfer;
                        //visitor.CalledTime = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Saves a visitor transfer
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<bool> SaveTransferRecord(int visitorId)
        {
            try
            {
                Visitor visitor = await GetVisitorByVisitorId(visitorId);

                if (visitor != null)
                {
                    VisitorTransferLog transfer = new();

                    transfer.VisitorId = visitor.Id;

                    if (visitor.VisitDepartmentId != null)
                        transfer.Department = (sbyte)visitor.VisitDepartmentId;

                    transfer.Location = visitor.Location;
                    transfer.VisitCategoryId = visitor.VisitCategoryId;

                    _context.VisitorTransfersLog.Add(transfer);
                    await _context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Change visitor reason, update department, reset called time, put visitor back
        /// into waiting queue. Agent stats should be updated 
        /// before this call
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<bool> ChangeVisitorReason(ulong categoryId, int visitorId)
        {
            try
            {
                Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId);

                if (visitor != null)
                {
                    visitor.VisitDepartmentId = await LookupDepartmentId(categoryId);
                    visitor.VisitCategoryId = categoryId;
                    //visitor.IsTransfer = false;
                    visitor.StatusName = "WAITING";
                    visitor.CalledTime = null;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Update agent status and counter status
        /// </summary>
        /// <param name="host"></param>
        /// <param name="agentName"></param>
        /// <param name="isAvailable"></param>
        /// <returns></returns>
        public async Task SetCounterStatus(string host, string agentName, bool isAvailable)
        {
            try
            {
                var counter = await _context.Counters
                    .Where(p => p.Host == host)
                    .FirstOrDefaultAsync<Counter>();

                Agent agent = await _context.Agents
                    .FirstOrDefaultAsync<Agent>(p => p.AuthName == agentName);

                if (counter != null)
                {
                    if (agent != null)
                        if (agent.Counter == host)
                            agent.StatusName = agent.StatusName == "AVAILABLE" ? "UNAVAILABLE" : "AVAILABLE";

                    counter.IsAvailable = isAvailable;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// GetVisitorByAgent
        /// </summary>
        /// <param name="agent_name"></param>
        /// <returns></returns>
        public async Task<Visitor> GetVisitorByAgent(string agent_name)
        {
            Agent agent = await AgentContext(agent_name);
            if (agent != null)
            {
                Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == agent.VisitorId);

                return visitor;
            }
            return null;
        }

        /// <summary>
        /// GetVisitorByCounter
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<Visitor> GetVisitorByCounter(string groupName)
        {
            Agent agent = await GetCounterFromGroupName(groupName);
            if (agent != null)
            {
                Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.AssignedCounter == agent.Counter);

                return visitor;
            }
            return null;
        }

        /// <summary>
        /// GetVisitorByVisitorId
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<Visitor> GetVisitorByVisitorId(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

            return visitor;
        }

        /// <summary>
        /// GetAssignedCounter
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<string> GetAssignedCounter(int visitorId)
        {
            return await _context.Visitors
                .Where(p => p.Id == visitorId)
                .Select(p => p.AssignedCounter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// CloseCall
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public async Task<bool> CloseCall(Visitor visitor)
        {
            try
            {
                // save visitor Id before removal
                int visitor_id = visitor.Id;

                // delete visitor
                _context.Visitors.Remove(visitor);

                // set counter available
                Counter counter = await _context.Counters
                    .Where(p => p.Host == visitor.AssignedCounter)
                    .FirstOrDefaultAsync();

                if (counter != null)
                    counter.IsAvailable = true;

                // remove visitor id from agent if exists
                Agent agent = await _context.Agents
                    .FirstOrDefaultAsync<Agent>(p => p.VisitorId == visitor_id);

                if (agent != null)
                    agent.VisitorId = 0;

                // commit all
                await _context.SaveChangesAsync();

                // verify visitor is deleted
                visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitor_id && p.StatusName == "CALLED");

                return visitor == null;

            }
            catch (Exception)
            {
                // Debug.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// SetVisitorCalled
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<Visitor> SetVisitorCalled(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

            if (visitor != null)
            {
                visitor.StatusName = "CALLED";
                await _context.SaveChangesAsync();

                visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitor.Id && p.StatusName == "CALLED");
            }
            return visitor;
        }

        /// <summary>
        /// SetVisitorArrivedValue
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<string> SetVisitorArrivedValue(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

            if (visitor != null)
            {
                visitor.StatusName = "ARRIVED";
                // managers will receive email if visitor has not been called by a certain time.
                visitor.CalledTime = DateTime.Now;
                await _context.SaveChangesAsync();

                VisitorStatus status = await _context.VisitorStatuses
                    .FirstOrDefaultAsync<VisitorStatus>(p => p.StatusName == "ARRIVED");

                return status.StatusDescription;
                //return Tuple.Create(status.StatusDescription, visitor.Id);
            }
            return null;
        }

        public async Task<string> PurgeVisitorInQueue(int visitorId)
        {
            try
            {
                Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId && p.StatusName == "WAITING");

                if (visitor != null)
                {
                    _context.Visitors.Remove(visitor);
                    await _context.SaveChangesAsync();
                    return "success";
                }
                else
                {
                    return "Visitor was not found.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// GetGroupDiplaysByLocation
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<List<GroupDevice>> GetGroupDiplaysByLocation(int location)
        {
            return await GetDisplaysAtLocation(location);
        }

        /// <summary>
        /// GetAssignedCounterHost
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task<Counter> GetAssignedCounterHost(string host)
        {
            Counter counter = await (from v in _context.Visitors
                                     join c in _context.Counters on v.AssignedCounter equals c.Host
                                     where c.Host == host
                                     select c).FirstOrDefaultAsync();

            return counter;
        }

        /// <summary>
        /// GetVisitorByHost
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public async Task<Visitor> GetVisitorByHost(string host)
        {
            Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.AssignedCounter == host);

            return visitor;
        }

        /// <summary>
        /// GetVisitorById
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task<Visitor> GetVisitorById(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId);

            return visitor;
        }

        /// <summary>
        /// Do all visitor processes for display
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public async Task<Queue> GetQueueByVisitor(Visitor visitor)
        {
            // Parses duplicate visitor names
            Queue queue = await DisplayDisambiguatorVisitor(visitor);
            if (queue != null)
                return queue;

            return null;
        }

        /// <summary>
        /// Agent metrics, unformatted
        /// Calculate accurate metric data,
        /// Example: if today is 12/7/2021 and the last entry 
        /// for user was 11/9/2021 then today, wtd, mtd should be zero.
        /// Adds totals row
        /// </summary>
        /// <returns></returns>
        public async Task<List<AgentMetricDetail>> GetAllAgentMetrics()
        {
            try
            {
                // list to return
                List<AgentMetricDetail> m = new List<AgentMetricDetail>();

                // Get agent metrics

                List<AgentMetricDetail> query = await (from v in _context.VsisUsers
                                                       join a in _context.AgentMetrics on v.AuthName equals a.AuthName
                                                       select new AgentMetricDetail
                                                       {
                                                           AuthName = v.AuthName,
                                                           FullName = v.FullName,
                                                           VisitorsToday = (a == null ? 0 : a.VisitorsToday),
                                                           VisitorsWtd = (a == null ? 0 : a.VisitorsWtd),
                                                           VisitorsMtd = (a == null ? 0 : a.VisitorsMtd),
                                                           VisitorsYtd = (a == null ? 0 : a.VisitorsYtd),
                                                           CallTimeToday = (a == null ? 0 : a.CallTimeToday),
                                                           CallTimeWtd = (a == null ? 0 : a.CallTimeWtd),
                                                           CallTimeMtd = (a == null ? 0 : a.CallTimeMtd),
                                                           CallTimeYtd = (a == null ? 0 : a.CallTimeYtd),
                                                           Created = a.Created

                                                       }).ToListAsync();

                List<AgentMetric> agents = await _context.AgentMetrics.ToListAsync();

                // Get today's date
                DateTime dt = DateTime.Now;

                // Get today date parts
                int today_wtd = enUS.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int today_today = dt.Day;
                int today_mtd = dt.Month;
                int today_ytd = dt.Year;

                // Add to list
                foreach (var a in query)
                {
                    // Get today
                    DateTime today_dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Utc);

                    // Get last metrics entry for agent
                    DateTime last_dt = TimeZoneInfo.ConvertTimeFromUtc(a.Created, TimeZoneInfo.Utc);

                    // Find difference between today and agent last entry
                    DateTimeOffset dtoToday = new DateTimeOffset(today_dt, TimeSpan.Zero);
                    DateTimeOffset dtoLast = new DateTimeOffset(last_dt, TimeSpan.Zero);

                    // This is the difference between today and agent last entry in seconds since epoch
                    long difference = (dtoToday.ToUnixTimeSeconds() - dtoLast.ToUnixTimeSeconds());

                    // Get difference in days
                    long days = difference / (24 * 3600);

                    // Get difference in hours
                    long seconds = difference % (24 * 3600);
                    long hours = seconds / 3600;

                    // Get difference in seconds
                    seconds %= 3600;
                    long minutes = seconds / 60;
                    seconds %= 60;

                    // declare vars for metric now date
                    int todayVal = 0, wtdVal = 0, mtdVal = 0, ytdVal = 0;
                    // declare vars for call time
                    double todayCall = 0, wtdCall = 0, mtdCall = 0, ytdCall = 0;

                    // Get last agent metric entry date parts.
                    int lweeknumber = enUS.Calendar.GetWeekOfYear(last_dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    int ltoday = last_dt.Day;
                    int lmonth = last_dt.Month;
                    int lyear = last_dt.Year;

                    // Take the value if equal
                    //
                    var takeToday = days == 0 && dtoToday.Month == lmonth && dtoToday.Year == lyear;
                    if (takeToday)
                    {
                        todayVal = a.VisitorsToday;
                        todayCall = a.CallTimeToday;
                    }
                    var takeWtd = today_wtd == lweeknumber && dtoToday.Month == lmonth && dtoToday.Year == lyear;
                    if (takeWtd)
                    {
                        wtdVal = a.VisitorsWtd;
                        wtdCall = a.CallTimeWtd;
                    }
                    var takeMtd = today_mtd == lmonth && dtoToday.Year == lyear;
                    if (takeMtd)
                    {
                        mtdVal = a.VisitorsMtd;
                        mtdCall = a.CallTimeMtd;
                    }
                    if (today_ytd == lyear)
                    {
                        ytdVal = a.VisitorsYtd;
                        ytdCall = a.CallTimeYtd;
                    }

                    m.Add(new AgentMetricDetail
                    {
                        AuthName = a.AuthName,
                        FullName = a.FullName,
                        VisitorsToday = todayVal,
                        VisitorsWtd = wtdVal,
                        VisitorsMtd = mtdVal,
                        VisitorsYtd = ytdVal,
                        CallTimeToday = todayCall,
                        CallTimeWtd = wtdCall,
                        CallTimeMtd = mtdCall,
                        CallTimeYtd = ytdCall,
                    });
                }

                // Append a totals row
                m.Add(new AgentMetricDetail
                {
                    FullName = "Totals",
                    VisitorsToday = m.Select(p => p.VisitorsToday).Sum(),
                    VisitorsWtd = m.Select(p => p.VisitorsWtd).Sum(),
                    VisitorsMtd = m.Select(p => p.VisitorsMtd).Sum(),
                    VisitorsYtd = m.Select(p => p.VisitorsYtd).Sum(),
                    CallTimeToday = m.Select(p => p.CallTimeToday).Sum(),
                    CallTimeWtd = m.Select(p => p.CallTimeWtd).Sum(),
                    CallTimeMtd = m.Select(p => p.CallTimeMtd).Sum(),
                    CallTimeYtd = m.Select(p => p.CallTimeYtd).Sum()
                });
                return m;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Category metrics, unformatted
        /// Calculate accurate metric data,
        /// Example: if today is 12/7/2021 and the last entry 
        /// for user was 11/9/2021 then today, wtd, mtd should be zero.
        /// Adds totals row
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryMetric>> GetCategoryMetrics()
        {
            try
            {
                // list to return
                List<CategoryMetric> m = new List<CategoryMetric>();

                // Get category metrics

                List<CategoryMetric> query = await (from c in _context.CategoryMetrics
                                                    select new CategoryMetric
                                                    {
                                                        Category = c.Category,
                                                        Description = c.Description,
                                                        CategoryToday = (c == null ? 0 : c.CategoryToday),
                                                        CategoryWtd = (c == null ? 0 : c.CategoryWtd),
                                                        CategoryMtd = (c == null ? 0 : c.CategoryMtd),
                                                        CategoryYtd = (c == null ? 0 : c.CategoryYtd),
                                                        CallTimeToday = (c == null ? 0 : c.CallTimeToday),
                                                        CallTimeWtd = (c == null ? 0 : c.CallTimeWtd),
                                                        CallTimeMtd = (c == null ? 0 : c.CallTimeMtd),
                                                        CallTimeYtd = (c == null ? 0 : c.CallTimeYtd),
                                                        Created = c.Created
                                                    }).ToListAsync();

                // Get today's date
                DateTime dt = DateTime.Now;

                // Get today date parts
                int today_wtd = enUS.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int today_today = dt.Day;
                int today_mtd = dt.Month;
                int today_ytd = dt.Year;

                // Add to list
                foreach (var a in query)
                {
                    // Get today
                    DateTime today_dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Utc);

                    // Get last metrics entry for agent
                    DateTime last_dt = TimeZoneInfo.ConvertTimeFromUtc(a.Created, TimeZoneInfo.Utc);

                    // Find difference between today and agent last entry
                    DateTimeOffset dtoToday = new DateTimeOffset(today_dt, TimeSpan.Zero);
                    DateTimeOffset dtoLast = new DateTimeOffset(last_dt, TimeSpan.Zero);

                    // This is the difference between today and agent last entry in seconds since epoch
                    long difference = (dtoToday.ToUnixTimeSeconds() - dtoLast.ToUnixTimeSeconds());

                    // Get difference in days
                    long days = difference / (24 * 3600);

                    // Get difference in hours
                    long seconds = difference % (24 * 3600);
                    long hours = seconds / 3600;

                    // Get difference in seconds
                    seconds %= 3600;
                    long minutes = seconds / 60;
                    seconds %= 60;

                    // declare vars for metric now date
                    int todayVal = 0, wtdVal = 0, mtdVal = 0, ytdVal = 0;
                    // declare vars for call time
                    double todayCall = 0, wtdCall = 0, mtdCall = 0, ytdCall = 0;

                    // Get last agent metric entry date parts.
                    int lweeknumber = enUS.Calendar.GetWeekOfYear(last_dt, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                    int ltoday = last_dt.Day;
                    int lmonth = last_dt.Month;
                    int lyear = last_dt.Year;

                    // Take the value if equal
                    //
                    var takeToday = days == 0 && dtoToday.Month == lmonth && dtoToday.Year == lyear;
                    if (takeToday)
                    {
                        todayVal = a.CategoryToday;
                        todayCall = a.CallTimeToday;
                    }
                    var takeWtd = today_wtd == lweeknumber && dtoToday.Month == lmonth && dtoToday.Year == lyear;
                    if (takeWtd)
                    {
                        wtdVal = a.CategoryWtd;
                        wtdCall = a.CallTimeWtd;
                    }
                    var takeMtd = today_mtd == lmonth && dtoToday.Year == lyear;
                    if (takeMtd)
                    {
                        mtdVal = a.CategoryMtd;
                        mtdCall = a.CallTimeMtd;
                    }
                    if (today_ytd == lyear)
                    {
                        ytdVal = a.CategoryYtd;
                        ytdCall = a.CallTimeYtd;
                    }

                    m.Add(new CategoryMetric
                    {
                        Category = a.Category,
                        Description = a.Description,
                        CategoryToday = todayVal,
                        CategoryWtd = wtdVal,
                        CategoryMtd = mtdVal,
                        CategoryYtd = ytdVal,
                        CallTimeToday = todayCall,
                        CallTimeWtd = wtdCall,
                        CallTimeMtd = mtdCall,
                        CallTimeYtd = ytdCall,
                    });
                }

                // Append a totals row
                m.Add(new CategoryMetric
                {
                    Description = "Totals",
                    CategoryToday = m.Select(p => p.CategoryToday).Sum(),
                    CategoryWtd = m.Select(p => p.CategoryWtd).Sum(),
                    CategoryMtd = m.Select(p => p.CategoryMtd).Sum(),
                    CategoryYtd = m.Select(p => p.CategoryYtd).Sum(),
                    CallTimeToday = m.Select(p => p.CallTimeToday).Sum(),
                    CallTimeWtd = m.Select(p => p.CallTimeWtd).Sum(),
                    CallTimeMtd = m.Select(p => p.CallTimeMtd).Sum(),
                    CallTimeYtd = m.Select(p => p.CallTimeYtd).Sum()
                });

                return m;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<VisitorDetail>> GetOneVisitor(int visitorId)
        {
            var query = await (from v in _context.Visitors
                               where v.Id == visitorId
                               // join c in _context.Counters on v.AssignedCounter equals c.Host
                               join d in _context.Categories on v.VisitCategoryId equals d.Id
                               join c in _context.Counters on v.AssignedCounter equals c.Host into counterGroup
                               from cg in counterGroup.DefaultIfEmpty()
                               select new
                               {
                                   Id = v.Id,
                                   FirstName = v.FirstName,
                                   LastName = v.LastName,
                                   IsHandicap = v.IsHandicap,
                                   IsTransfer = v.IsTransfer,
                                   StatusName = v.StatusName,
                                   Location = v.Location,
                                   AssignedCounter = v.AssignedCounter,
                                   CounterDescription = cg.Description,
                                   VisitCategory = d.Description,
                                   VisitCategoryId = v.VisitCategoryId,
                                   Created = v.Created,
                                   CalledTime = v.CalledTime
                               }).ToListAsync();

            List<VisitorDetail> visitors = new();

            foreach (var p in query)
            {
                visitors.Add(new VisitorDetail
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsHandicap = p.IsHandicap,
                    IsTransfer = p.IsTransfer,
                    StatusName = p.StatusName,
                    Location = p.Location,
                    AssignedCounter = p.AssignedCounter,
                    CounterDescription = p.CounterDescription,
                    CategoryDescription = p.VisitCategory,
                    VisitCategoryId = p.VisitCategoryId,
                    Created = p.Created,
                    CalledTime = p.CalledTime
                });
            }

            return visitors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<VisitorDetail>> GetAllVisitors()
        {
            var query = await (from v in _context.Visitors
                               join d in _context.Categories on v.VisitCategoryId equals d.Id
                               join w in _context.WaitTimeNotifies on v.VisitCategoryId equals w.Category
                               join c in _context.Counters on v.AssignedCounter equals c.Host into counterGroup
                               from cg in counterGroup.DefaultIfEmpty()
                               select new
                               {
                                   Id = v.Id,
                                   FirstName = v.FirstName,
                                   LastName = v.LastName,
                                   IsHandicap = v.IsHandicap,
                                   IsTransfer = v.IsTransfer,
                                   StatusName = v.StatusName,
                                   Location = v.Location,
                                   AssignedCounter = v.AssignedCounter,
                                   CounterDescription = cg.Description,
                                   VisitCategory = d.Description,
                                   VisitCategoryId = d.Id,
                                   Created = v.Created,
                                   CalledTime = v.CalledTime,
                                   MaxWaitTime = w.MaxWaitTimeMinutes

                               }).ToListAsync();

            List<VisitorDetail> visitors = new();

            foreach (var p in query)
            {
                visitors.Add(new VisitorDetail
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsHandicap = p.IsHandicap,
                    IsTransfer = p.IsTransfer,
                    StatusName = p.StatusName,
                    Location = p.Location,
                    AssignedCounter = p.AssignedCounter,
                    CounterDescription = p.CounterDescription,
                    CategoryDescription = p.VisitCategory,
                    VisitCategoryId = p.VisitCategoryId,
                    Created = p.Created,
                    CalledTime = p.CalledTime,
                    MaxWaitTime = p.MaxWaitTime
                });
            }

            return visitors;
        }

        /// <summary>
        /// Get queue for display
        /// </summary>
        /// <returns>List<Queue></returns>
        public async Task<List<Queue>> GetQueueForDisplay(int location)
        {
            var exceptionList = new List<string> { "ARRIVED" };

            var query = await (from v in _context.Visitors
                               join c in _context.Counters on v.AssignedCounter equals c.Host into gj
                               from subc in gj.DefaultIfEmpty()
                               where !exceptionList.Contains(v.StatusName) && v.Location == location
                               select new
                               {
                                   Id = v.Id,
                                   FirstName = v.FirstName,
                                   LastName = v.LastName,
                                   IsHandicap = v.IsHandicap,
                                   IsTransfer = v.IsTransfer,
                                   Created = v.Created,
                                   StatusName = v.StatusName,
                                   AssignedCounter = subc.Description
                               }).ToListAsync();

            List<Visitor> visitors = new();

            foreach (var p in query)
            {
                visitors.Add(new Visitor
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsHandicap = p.IsHandicap,
                    IsTransfer = p.IsTransfer,
                    Created = p.Created,
                    StatusName = p.StatusName,
                    AssignedCounter = p.AssignedCounter
                });
            }

            return DisplayDisambiguator(visitors);
        }

        /// <summary>
        /// Modify visitor display name if any other visitors in queue have the same first name
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns>
        /// Unique DisplayName for visitor
        /// </returns>
        private async Task<Queue> DisplayDisambiguatorVisitor(Visitor visitor)
        {
            List<Queue> queue = await GetQueueForDisplay(visitor.Location);
            Queue queueItem = new();
            try
            {
                int n = 1;
                string dname = visitor.FirstName;
                string icon = "";

                // find duplicate first names in entire queue
                List<Queue> fn2 = queue.Where(x => x.DisplayName.Contains(visitor.FirstName)).ToList<Queue>();

                // suffix last name up to 2 characters to reduce duplication on display
                if (fn2.Count > 1)
                {
                    // take first initial if count is 1, or take first two initials
                    n = fn2.Count == 2 ? 1 : 2;
                    dname = visitor.FirstName + " " + visitor.LastName.Substring(0, n);
                }

                //if (visitor.IsHandicap)
                //    icon = "isa-handicap.png";

                if (visitor.IsTransfer)
                    icon = "transfer.png";

                // add item to queue
                queueItem.Id = visitor.Id;
                queueItem.DisplayName = dname.Trim();
                queueItem.Icon = icon;
                queueItem.AssignedCounter = visitor.AssignedCounter;
                queueItem.StatusName = visitor.StatusName;
                queueItem.QueueTimeStamp = visitor.Created;
                queueItem.IsTransfer = visitor.IsTransfer;
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
            return queueItem;
        }

        /// <summary>
        /// Modify all visitor display names to reduce ambiguity
        /// If any other visitors in queue have the same first name,
        /// include the last name initial starting with the second person
        /// Repeat when last name initial differs
        /// Data is sorted before evaluating
        /// Example:
        ///  George     (Lima, first person, so show first name only)
        ///  George S   (Smith, 2nd duplicate first name, show one character last name initial)
        ///  George Sp  (Spam, 3rd duplicate first name, show two character last name intials)
        ///  George W   (Wallace, 4th duplcate first name but different last name, show one character last name initial)

        /// </summary>
        /// <param name="visitor"></param>
        /// <returns>
        /// Reduced ambiguous list of DisplayNames
        /// </returns>
        private List<Queue> DisplayDisambiguator(List<Visitor> visitors)
        {
            List<Queue> queue = new();

            // temp order by first name before checking for duplicates
            visitors = new List<Visitor>(visitors.OrderBy(p => p.Created).ThenBy(p => p.FirstName).ThenBy(p => p.LastName));

            try
            {
                int n = 1;
                int x = 0;
                int temp_count = 0;
                string last_initial = "";
                string current_initial = "";

                foreach (var fn in visitors)
                {
                    string icon = "";

                    // show first name only when there are no duplicates
                    string dname = fn.FirstName;

                    // find duplicate first names
                    List<Visitor> fn2 = visitors.Where(x => x.FirstName.Contains(fn.FirstName)).ToList<Visitor>();

                    // suffix last name up to 2 characters to reduce duplicates on display
                    if (fn2.Count > 1)
                    {
                        // find last names with same first letter
                        if (fn.LastName.Length >= n)
                        {
                            // substring start index must be > 0 and =< lastname length
                            List<Visitor> ln = fn2.Where(x => x.LastName.StartsWith(fn.LastName.Substring(0, n))).ToList<Visitor>();

                            current_initial = fn.LastName.Substring(0, 1);

                            // if new group
                            if (last_initial != current_initial)
                                if (last_initial.Length > 0)
                                    if (ln.Count == 1 && last_initial != current_initial)
                                        x = 1;
                            if (x == ln.Count && last_initial == current_initial)
                                x = 2;
                            if (temp_count == ln.Count && last_initial != current_initial)
                                x = 1;

                            if (x > 3)
                                x = 1;
                            if (x == 2)
                                x = 2;

                            temp_count = ln.Count;
                            last_initial = fn.LastName.Substring(0, 1);

                            if (x > 1)
                                x = 2;
                            if (n > 2)
                                n = 1;
                        }

                        if (fn.LastName.Length > n)
                            dname = (fn.FirstName + " " + fn.LastName.Substring(0, x)).Trim();

                        x++;
                    }
                    // remove handicap icon from display
                    // TODO: maybe show called icon

                    //if (fn.IsHandicap)
                    //    icon = "isa-handicap.png";

                    if (fn.IsTransfer)
                        icon = "transfer.png";

                    // add item to queue
                    queue.Add(new Queue
                    {
                        Id = fn.Id,
                        DisplayName = dname.Trim(),
                        Icon = icon,
                        AssignedCounter = fn.AssignedCounter,
                        IsTransfer = fn.IsTransfer,
                        StatusName = fn.StatusName,
                        QueueTimeStamp = fn.Created
                    });
                    //queue.ForEach(value => { Debug.WriteLine(value.DisplayName); });
                }

                // reorder by sign in time ascending
                // gwb, changed ordering and moved to return
                // queue = new List<Queue>(queue.OrderBy(x => x.QueueTimeStamp));

            }
            catch (Exception) { }
            finally
            {
                // order by called, transferred, then time
                //queue = new List<Queue>(queue.OrderByDescending(x => x.StatusName == "CALLED").ThenBy(x => x.IsTransfer).ThenBy(x => x.QueueTimeStamp));

                queue = new List<Queue>(queue.OrderByDescending(x => x.IsTransfer).ThenBy(x => x.QueueTimeStamp));

            }
            return queue;
        }
    }

    /// <summary>
    /// VisitorDetail
    /// </summary>
    public class VisitorDetail : Visitor
    {
        public string CounterDescription { get; set; }
        public string CategoryDescription { get; set; }
        //public bool AgentIsAvailable { get; set; }
        public int MaxWaitTime { get; set; }
    }

    public class AgentMetricDetail : AgentMetric
    {
        public string FullName { get; set; }
    }

    public class VsisUserDetail : VsisUser
    {
        public string DepartmentName { get; set; }
        // categories Id
        //public ulong Id { get; set; }
        public int Categories { get; set; }
    }
    // end namespace
}
