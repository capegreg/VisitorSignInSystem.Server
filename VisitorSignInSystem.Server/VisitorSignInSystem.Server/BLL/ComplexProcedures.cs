using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisitorSignInSystem.Server.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace VisitorSignInSystem.Server.BLL
{
    public class ComplexProcedures
    {
        private readonly vsisdataContext _context;

        public ComplexProcedures(vsisdataContext context)
        {
            _context = context;
        }

        // Make this async Install-Package System.Linq.Async -Version 5.0.0
        // https://docs.microsoft.com/en-us/ef/core/miscellaneous/async
        
        public async Task<List<Counter>> GetAllCounters()
        {
            List<Counter> counters = await _context.Counters
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

        public async Task<List<string>> GetAvailableAgents()
        {
            List<string> agents = await _context.Agents
                .Where(p => p.StatusName == "AVAILABLE")
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        public async Task<int> GetAvailableAgentsByCategory(ulong category, int location)
        {
            var param1 = new MySqlConnector.MySqlParameter("@v_cat", category);
            var param2 = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<Agent> agents = await _context.Agents
                .FromSqlRaw($"CALL vsisdata.getAvailableAgentsByCategory(@v_cat, @v_location);",
                new object[] { param1, param2 })
                .ToListAsync();

            return agents.Count;
        }

        public async Task<List<string>> GetUnAvailableAgents()
        {
            List<string> agents = await _context.Agents
                .Where(p => p.StatusName == "UNAVAILABLE")
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        public async Task<List<string>> GetAllAgents()
        {
            List<string> agents = await _context.Agents
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        public async Task<List<string>> GetAgentNames()
        {
            List<string> agents = await _context.Agents
                .Select(p => p.AuthName).ToListAsync();

            return agents;
        }

        public async Task<WaitTimeNotify> GetMaxWaitTimeByCategory(ulong category)
        {
            return await _context.WaitTimeNotifies
                .Where(p => p.Category == category)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Agent>> GetOtherAgentsInCall(string groupName)
        {
            return await _context.Agents
                .Where<Agent>(p => p.AuthName != groupName && p.VisitorId != 0).ToListAsync();
        }

        public async Task<Agent> GetAgentByVisitorId(int visitorId)
        {
            return await _context.Agents
                .FirstOrDefaultAsync<Agent>(p => p.VisitorId == visitorId);
        }

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

        public async Task<List<Counter>> GetCountersByCategory(ulong category, int location)
        {
            var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
            var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<Counter> counters = await _context.Counters
                .FromSqlRaw("CALL vsisdata.getCountersByCategory(@v_cat, @v_location);",
                new object[] { cat, loc })
                .ToListAsync();

            return counters;
        }

        public async Task SaveNewVisitor(Visitor visitor)
        {
            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();
        }


        public async Task SaveAgentMetrics(string groupName, int visitorId)
        {
            try
            {
                Visitor v = await GetVisitorToClose(visitorId);
                if (v != null)
                {
                    string auth_agent = groupName;

                    // get agent who served visitor

                    Agent agent = await GetAgentByVisitorId(visitorId);
                    if (agent != null)
                        auth_agent = agent.AuthName;

                    VisitorMetric m = new VisitorMetric();
                    m.Agent = auth_agent;
                    m.Kiosk = v.Kiosk;
                    m.AssignedCounter = v.AssignedCounter;
                    m.Created = v.Created;
                    m.IsHandicap = v.IsHandicap;
                    m.Location = v.Location;
                    m.VisitCategoryId = v.VisitCategoryId;
                    m.CallDuration = DateTime.Now;

                    _context.VisitorMetrics.Add(m);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

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

        public async Task<string> GetCategory(ulong id)
        {
            var query = from p in _context.Categories
                        where p.Id == id
                        select p.Description;

            return await query.SingleAsync();
        }

        public async Task<int> QueueCount()
        {
            var query = from p in _context.Visitors
                        where p.StatusName == "WAITING"
                        select p.Id;

            return await query.CountAsync();
        }

        public async Task<int> QueueCountByAgent(string agent_name)
        {
            var query = from p in _context.Visitors
                        where p.StatusName == "WAITING"
                        select p.Id;

            return await query.CountAsync();
        }

        public async Task<List<Visitor>> GetAllVisitorsTaken()
        {
            return await _context.Visitors
                .Where(p => p.StatusName == "TAKEN").ToListAsync();
        }

        public async Task<AgentMetric> GetAgentMetrics(string agentName)
        {
            return await _context.AgentMetrics
                .Where(p => p.AuthName == agentName).FirstOrDefaultAsync();
        }

        //public async Task<int> AvailableHandicapCountersCount(int location)
        //{
        //    var query = from p in _context.Counters
        //                where p.Location == location && p.IsHandicap == true && p.IsAvailable == true
        //                select p.CounterNumber;

        //    return await query.CountAsync();
        //}

        public async Task<int> GetAvailableCountersCount(string groupName, int location)
        {
            var query = from p in _context.Counters
                        where p.Location == location && p.IsAvailable == true
                        select p.CounterNumber;

            return await query.CountAsync();
        }

        public async Task<int> GetQueueCountByAgent(string groupName, int location)
        {
            string sql = "";

            Counter counter = await _context.Counters
            .FirstOrDefaultAsync(p => p.Host == groupName);

            string agent_name = await _context.Agents
               .Where(p => p.Counter == groupName)
               .Select(p => p.AuthName).FirstOrDefaultAsync();

            if (agent_name != "")
            {
                // default sql
                sql = "vsisdata.getVisitorsByAgent(@v_auth_name, @v_location);";

                if (counter != null)
                {
                    if (agent_name != null)
                        groupName = agent_name;

                    if (counter.IsHandicap)
                        sql = "vsisdata.getHandicapVisitorsByAgent(@v_auth_name, @v_location);";
                }

                var agent = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
                var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

                List<Visitor> vis = await _context.Visitors
                    .FromSqlRaw($"CALL {sql};",
                    new object[] { agent, loc })
                    .ToListAsync();

                return vis.Count;
            }
            return 0;
        }

        public async Task<Visitor> GetNextWaitingVisitor(string groupName, int location, string groupType)
        {
            Visitor visitor = new Visitor();

            var param1 = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
            var param2 = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            // default groupType to Agent
            string sql = "vsisdata.getNextInLineByAgent(@v_auth_name, @v_location);";

            switch (groupType)
            {
                case "Counter":

                    Counter counter = await GetCounterContext(groupName);
                    param1 = new MySqlConnector.MySqlParameter("@v_cat", counter.Category);

                    sql = "vsisdata.getNextInLineByCategory(@v_cat, @v_location);";
                    break;
            }

            List<Visitor> vis = await _context.Visitors
                .FromSqlRaw($"CALL {sql};",
                new object[] { param1, param2 })
                .ToListAsync();

            if (vis.Count > 0)
            {

                visitor = vis[0];

                if (visitor != null)
                {
                    // mark visitor called
                    visitor.StatusName = "TAKEN";
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                visitor = null;
            }
            return visitor;
        }

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

        public async Task<List<Category>> GetCategories(int location)
        {
            List<Category> cats = new List<Category>();
            try
            {
                cats = await _context.Categories
                    .Where(p => p.Location == location)
                    .ToListAsync<Category>();
            }
            catch (Exception) {}
            return cats;
        }

        public async Task<List<GroupDevice>> GetDisplaysAtLocation(int location)
        {
            List<GroupDevice> devices = new List<GroupDevice>();
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

        public async Task<List<Location>> GetAllLocations()
        {
            List<Location> locations = new List<Location>();
            try
            {
                locations = await _context.Locations
                    .Where(p => p.Open == true)
                    .ToListAsync<Location>();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
            return locations;
        }

        public async Task<Agent> AgentContext(string groupName)
        {
            return await _context.Agents
            .FirstOrDefaultAsync<Agent>(p => p.AuthName == groupName);
        }

        public async Task<string> GetAgentCounter(string agent_name)
        {
            return await _context.Agents
                .Where(p => p.AuthName == agent_name)
                .Select(p => p.Counter).FirstOrDefaultAsync();
        }

        public async Task<bool?> IsCounter(string groupName)
        {
            try
            {
                var c = await (from v in _context.Counters
                               where v.Host == groupName
                               select v).ToListAsync();

                return c.Count > 0;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<VsisUser>> AgentUserNames(string groupName)
        {
            return await (from v in _context.VsisUsers
                          join c in _context.Agents on v.AuthName equals c.AuthName
                          select v).ToListAsync();
        }


        public async Task<List<VsisUser>> GetAgentsByCategory(ulong category, int location)
        {
            var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
            var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

            List<VsisUser> vis = await _context.VsisUsers
                .FromSqlRaw("CALL vsisdata.getAgentsByCategory(@v_cat, @v_location);",
                new object[] { cat, loc })
                .ToListAsync();

            return vis;
        }

        public async Task<string> GetAgentByCategory(string groupName, ulong category, int location)
        {
            try
            {
                var authname = new MySqlConnector.MySqlParameter("@v_auth_name", groupName);
                var cat = new MySqlConnector.MySqlParameter("@v_cat", category);
                var loc = new MySqlConnector.MySqlParameter("@v_location", (sbyte)location);

                List<Agent> agent = await _context.Agents
                    .FromSqlRaw("CALL vsisdata.getAgentByCategory(@v_auth_name, @v_cat, @v_location);",
                    new object[] { authname, cat, loc })
                    .ToListAsync();

                if (agent.Count>0)
                   return agent[0].AuthName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return "";
        }

        public async Task<Agent> GetCounterFromGroupName(string groupName)
        {
            return await _context.Agents
                .FirstOrDefaultAsync(p => p.Counter == groupName);
        }

        public async Task<VsisUser> UserContext(string groupName)
        {
            return await _context.VsisUsers
            .FirstOrDefaultAsync<VsisUser>(p => p.AuthName == groupName);
        }

        public async Task<Counter> GetCounterContext(string counter_host)
        {
            Counter counter = await _context.Counters
            .FirstOrDefaultAsync<Counter>(p => p.Host == counter_host);

            Agent agent = await _context.Agents
                .FirstOrDefaultAsync<Agent>(p => p.Counter == counter_host);

            if(agent != null)
            {
                bool is_available = agent.StatusName == "AVAILABLE";
                counter.IsAvailable = is_available;
                await _context.SaveChangesAsync();
            }

            return counter;
        }

        public async Task<bool> IsValidCounter(string counter_host)
        {
            var counter = await _context.Counters
            .FirstOrDefaultAsync(p => p.Host == counter_host);

            return counter != null;
        }

        //public async Task<Agent> GetAuthAgent(string agent_name)
        //{
        //    Agent agent = await _context.Agents
        //        .Where(p => p.AuthName == agent_name)
        //        .Select(p => new Agent()
        //        {
        //            AuthName = p.AuthName,
        //            Categories = p.Categories,
        //            StatusName = p.StatusName
        //        }).FirstOrDefaultAsync();

        //    return agent;
        //}


        public async Task<string> GetAgentStatus(string agent_name)
        {
            Agent agent = await AgentContext(agent_name);
            if (agent != null)
            {
                return agent.StatusName;

                //Agent agent = await _context.Agents
                //.Where(p => p.AuthName == agent_name)
                //.Select(p => new Agent()
                //{
                //    StatusName = p.StatusName
                //}).FirstOrDefaultAsync();
            }
            return "";
        }

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

        public async Task SetCounterAvailable(string agentName, string counter_host)
        {
            // get counter context for update
            Counter counter = await _context.Counters
                .FirstOrDefaultAsync(p => p.Host == counter_host);

            if (counter != null)
            {
                await SetAgentCounter(agentName, counter.Host);
            }
        }

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

        public async Task SetAgentStatus(string agent_name, string statusName)
        {
            var agent = await _context.Agents
                              .Where(p => p.AuthName == agent_name)
                              .FirstOrDefaultAsync();

            agent.StatusName=statusName;
            await _context.SaveChangesAsync();
        }

        public async Task SetAgentCounter(string agent_name, string counter_host)
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
        
        public async Task<bool> CancelCallVisitor(int visitorId)
        {
            /*
             * Get visitor assigned to agent
             * Reset visitor to waiting
             * Check if counter was assigned and make it available
             * */
            bool success = false;

            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

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

        public async Task SetCounterStatus(string host, string agentName, bool isAvailable)
        {
            var counter = await _context.Counters
                .Where(p => p.Host == host)
                .FirstOrDefaultAsync<Counter>();

            Agent agent = await _context.Agents
                .FirstOrDefaultAsync<Agent>(p => p.AuthName == agentName);

            if (counter != null)
            {
                if(agent != null)
                    if(agent.Counter == host)
                        agent.StatusName = agent.StatusName == "AVAILABLE" ? "UNAVAILABLE" : "AVAILABLE";
                
                counter.IsAvailable = isAvailable;
                await _context.SaveChangesAsync();

                
            }
        }

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

        public async Task<Visitor> GetVisitorByVisitorId(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

            return visitor;
        }

        public async Task<Visitor> GetVisitorToClose(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId);

            return visitor;
        }

        public async Task<string> GetAssignedCounter(int visitorId)
        {
            return await _context.Visitors
                .Where(p => p.Id == visitorId)
                .Select(p => p.AssignedCounter).FirstOrDefaultAsync();
        }

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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }
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

        public async Task<string> SetVisitorArrivedValue(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                .FirstOrDefaultAsync(p => p.Id == visitorId);

            if (visitor != null)
            {
                visitor.StatusName = "ARRIVED";
                visitor.CalledTime = DateTime.Now;
                await _context.SaveChangesAsync();

                VisitorStatus status = await _context.VisitorStatuses
                    .FirstOrDefaultAsync<VisitorStatus>(p => p.StatusName == "ARRIVED");

                return status.StatusDescription;
                //return Tuple.Create(status.StatusDescription, visitor.Id);
            }
            return null;
        }

        public async Task<List<GroupDevice>> GetGroupDiplaysByLocation(int location)
        {
            return await GetDisplaysAtLocation(location);
        }

        public async Task<Counter> GetAssignedCounterHost(string host)
        {
            Counter counter = await (from v in _context.Visitors
                               join c in _context.Counters on v.AssignedCounter equals c.Host
                               where c.Host == host
                               select c).FirstOrDefaultAsync();

            return counter;
        }

        public async Task<Visitor> GetVisitorByHost(string host)
        {
            Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.AssignedCounter == host);

            return visitor;
        }

        public async Task<Visitor> GetVisitorById(int visitorId)
        {
            Visitor visitor = await _context.Visitors
                    .FirstOrDefaultAsync(p => p.Id == visitorId);

            return visitor;
        }

        public async Task<Queue> GetQueueByVisitor(Visitor visitor)
        {
            Queue queue = await DisplayDisambiguatorVisitor(visitor);
            if (queue != null)
            {
                return queue;
            }
            return null;
        }

        public async Task<List<Queue>> GetQueue()
        {
            var query = await (from v in _context.Visitors
                               join c in _context.Counters on v.AssignedCounter equals c.Host into gj
                               from subc in gj.DefaultIfEmpty()
                               select new
                               {
                                   Id = v.Id,
                                   FirstName = v.FirstName,
                                   LastName = v.LastName,
                                   IsHandicap = v.IsHandicap,
                                   Created = v.Created,
                                   StatusName = v.StatusName,
                                   AssignedCounter = subc.Description
                               }).ToListAsync();

            List<Visitor> visitors = new List<Visitor>();

            foreach (var p in query)
            {
                visitors.Add(new Visitor
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsHandicap = p.IsHandicap,
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
            List<Queue> queue = await GetQueue();
            Queue queueItem = new Queue();
            try
            {
                int n = 1;
                string dname = visitor.FirstName;
                //string icon = "";

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
                //{
                //    icon = "isa-handicap.png";
                //}

                // add item to queue
                queueItem.Id = visitor.Id;
                queueItem.DisplayName = dname.Trim();
                queueItem.Icon = ""; // icon;
                queueItem.AssignedCounter = visitor.AssignedCounter;
                queueItem.StatusName = visitor.StatusName;
                queueItem.QueueTimeStamp = visitor.Created;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            List<Queue> queue = new List<Queue>();

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
                    //string icon = "";

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
                                    if (ln.Count ==1 && last_initial != current_initial)
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
                    // todo: maybe show called icon

                    //if (fn.IsHandicap)
                    //    icon = "isa-handicap.png";

                    // add item to queue
                    queue.Add(new Queue { 
                        Id = fn.Id, 
                        DisplayName = dname.Trim(), 
                        Icon = "", //icon, 
                        AssignedCounter = fn.AssignedCounter, 
                        StatusName = fn.StatusName,
                        QueueTimeStamp = fn.Created 
                    });
                    //queue.ForEach(value => { Debug.WriteLine(value.DisplayName); });
                }
                // reorder by sign in time ascending
                queue = new List<Queue>(queue.OrderBy(x => x.QueueTimeStamp));
            }
            catch (Exception ex) { 
                Debug.WriteLine(ex.Message); 
            }
            return queue;
        }
    }

    public class AgentItem : Agent
    {
        public string FullName { get; set; }
    }
}
