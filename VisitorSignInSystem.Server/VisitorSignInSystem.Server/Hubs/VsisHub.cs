
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VisitorSignInSystem.Server.BLL;
using System.Diagnostics;
using VisitorSignInSystem.Server.Models;

namespace VisitorSignInSystem.Server.Hubs
{
    //[Authorize("VSISAuthorizationPolicy")]
    public class vsisHub : Hub
    {
        private readonly vsisdataContext _context;
        
        private enum UserRoleTypes
        {
            Agent,
            Counter,
            Manager,
            SysAdmin
        }

        public enum AgentStatusTypes
        {
            All,
            Available,
            Unavailable
        }

        public vsisHub(vsisdataContext context)
        {
            _context = context;            
        }

        #region send messages

        // [HubMethodName("UpdateDisplayQueue")]
        public Task SendQueueAll(string groupName, List<Queue> queue)
        {
            return Clients.Group(groupName).SendAsync("QueueList", queue);
        }
        public Task SendQueueVisitor(string groupName, Queue queue)
        {
            return Clients.Group(groupName).SendAsync("QueueVisitor", queue);
        }
        public Task SendQueueCount(string groupName, int count)
        {
            return Clients.Group(groupName).SendAsync("QueueCount", count);
        }
        public Task SendCountersCount(string groupName, int count)
        {
            return Clients.Group(groupName).SendAsync("CountersCount", count);
        }
        public Task SendAgents(string groupName, List<VsisUser> agent_user)
        {
            return Clients.Group(groupName).SendAsync("AgentUserNames", agent_user);
        }
        public Task SendAgentContext(string groupName, Agent agentContext)
        {
            return Clients.Group(groupName).SendAsync("AgentContext", agentContext);
        }
        public Task SendUserContext(string groupName, VsisUser userContext)
        {
            return Clients.Group(groupName).SendAsync("UserContext", userContext);
        }
        public Task SendAuthCounter(string groupName, Counter counter)
        {
            return Clients.Group(groupName).SendAsync("AuthCounter", counter);
        }
        public Task SendOfficeLocations(string groupName, List<Location> locations)
        {
            return Clients.Group(groupName).SendAsync("OfficeLocations", locations);
        }
        public Task SendAgentsInCategory(string groupName, List<VsisUser> agents)
        {
            return Clients.Group(groupName).SendAsync("AgentsInCategory", agents);
        }
        private Task SendVisitorInfo(string groupName, Visitor visitor, string category_description)
        {
            return Clients.Group(groupName).SendAsync("VisitorInfo", visitor, category_description);
        }
        private Task SendPresentCallInfo(string groupName, Visitor visitor, string category_description, Counter counter)
        {
            return Clients.Group(groupName).SendAsync("PresentCallInfo", visitor, category_description, counter);
        }
        public Task SendAgentStatus(string groupName, string agentStatus)
        {
            return Clients.Group(groupName).SendAsync("AgentStatus", agentStatus);
        }
        public Task SendCounterStatus(string groupName, bool isAvailable)
        {
            return Clients.Group(groupName).SendAsync("CounterStatus", isAvailable);
        }
        public Task SendAllCounters(string groupName, List<Counter> counters)
        {
            return Clients.Group(groupName).SendAsync("CountersAllList", counters);
        }
        public Task SendAllAgents(string groupName, List<Agent> agents)
        {
            return Clients.Group(groupName).SendAsync("AgentsAllList", agents);
        }
        public Task SendAvailableCounters(string groupName, List<Counter> counters)
        {
            return Clients.Group(groupName).SendAsync("CountersAvailableList", counters);
        }
        public Task SendAgentNames(string groupName, List<string> agents)
        {
            return Clients.Group(groupName).SendAsync("AgentNames", agents);
        }
        public Task SendVisitorWasAdded(string groupName, bool visitorAdded)
        {
            return Clients.Group(groupName).SendAsync("VisitorWasAdded", visitorAdded);
        }
        private Task SendNotifyNewVisitor(string groupName, string fname, string lname, string category_descr)
        {
            return Clients.Group(groupName).SendAsync("NotifyAgentsCounters", fname, lname, category_descr);
        }
        private Task SendNextInLine(string groupName, Visitor visitor, string category_description)
        {
            return Clients.Group(groupName).SendAsync("NextInLine", visitor, category_description);
        }
        private Task SendVisitorCalled(string groupName, int id)
        {
            return Clients.Group(groupName).SendAsync("VisitorCalled", id);
        }
        private Task SendNotifyVisitorDisplay(string groupName, int id, string assignedCounter)
        {
            return Clients.Group(groupName).SendAsync("NotifyVisitorDisplay", id, assignedCounter);
        }
        private Task SendCancelCallSuccess(string groupName, bool isSuccess)
        {
            return Clients.Group(groupName).SendAsync("CancelCallSuccess", isSuccess);

        }
        private Task SendNotifyRemoveVisitorDisplay(string groupName, int id)
        {
            return Clients.Group(groupName).SendAsync("NotifyRemoveVisitorDisplay", id);
        }
        private Task SendAgentStats(string groupName, AgentMetric stats)
        {
            return Clients.Group(groupName).SendAsync("AgentStats", stats);
        }
        private Task SendVisitorArrived(string groupName, string visitorStatus)
        {
            return Clients.Group(groupName).SendAsync("VisitorArrived", visitorStatus);
        }
        private Task SendCallWasClosed(string groupName, bool isClosed)
        {
            return Clients.Group(groupName).SendAsync("CallWasClosed", isClosed);
        }
        private Task SendCounterAssigned(string groupName, Counter counter, int counters_count, int visitorId, string assigner)
        {
            return Clients.Group(groupName).SendAsync("CounterAssigned", counter, counters_count, visitorId, assigner);
        }
        private Task SendCategoriesToLocation(string groupName, List<Category> cats)
        {
            return Clients.Group(groupName).SendAsync("Categories", cats);
        }
       
        #endregion

        #region received messages

        public async Task AddVisitor(string groupName, Visitor visitor)
        {           
            if (visitor != null)
            {
                ComplexProcedures c = new ComplexProcedures(_context);
                await c.SaveNewVisitor(visitor);

                // goes to kiosk
                await SendVisitorWasAdded(groupName, true);

                // goes to display
                await SendVisitorToDisplays(visitor.Location, visitor);

                // grab cat description for notification
                string category_descr = await c.GetCategory(visitor.VisitCategoryId);

                foreach (var item in ConnectedUser.Ids)
                {
                    // queue goes to all connected
                    await MessageQueueCount(item.Value, visitor.Location);

                    bool skipNotify = false;
                    string auth_name = item.Value;

                    // change auth_name if agent is at counter
                    Agent agent = await c.GetCounterFromGroupName(item.Value);
                    if (agent != null)
                        auth_name = agent.AuthName;

                    // get agent in category
                    string ausr = await c.GetAgentByCategory(auth_name, visitor.VisitCategoryId, visitor.Location);

                    // notifications go to agents and counters
                    if (ausr == "")
                        skipNotify = true;

                    if (!skipNotify)
                        await SendNotifyNewVisitor(item.Value, visitor.FirstName, visitor.LastName, category_descr);
                }

                // Get visitor category description to use in emails
                string cat_descr = await c.GetCategory(visitor.VisitCategoryId);

                // Get the max wait time and manager mail properties for category
                WaitTimeNotify notifer = await c.GetMaxWaitTimeByCategory(visitor.VisitCategoryId);

                if (notifer != null)
                {
                    // Send mail to manager if no agents are available to take call
                    int available_agents = await c.GetAvailableAgentsByCategory(visitor.VisitCategoryId, visitor.Location);
                    if (available_agents == 0)
                        SendNoAvailableAgentsMail(notifer, visitor, cat_descr);

                    // Create a timer reference for visitor
                    VisitorWaitingTimer visitorWait = new VisitorWaitingTimer();
                    visitorWait.StartVisitorTimer(visitor, cat_descr, notifer.MaxWaitTimeMinutes, notifer.Mail);
                    if (!WaitedVisitors.visitorsWaiting.ContainsKey(visitor.Id))
                        WaitedVisitors.visitorsWaiting.Add(visitor.Id, visitorWait);
                }
            }
        }

        private void SendNoAvailableAgentsMail(WaitTimeNotify notifer, Visitor visitor, string cat_descr)
        {
            var header = $"<tr><td valign=\"top\" style=\"border: 1px solid #0A4370;margin:5px;padding:10px;color:#D01110;text-align:center;\"><h2>No Agents are available to service {cat_descr}</h2></td></tr>";

            var body = "<table cellpadding=\"0\" cellspacing=\"1\" border=\"0\" style=\"font-size:16px;color:#0A4370;width:90%;\">";
            body += header;
            body += $"<tr><td valign=\"top\" style=\"margin: 6px;padding: 10px;\"><h3><li>{visitor.FirstName} {visitor.LastName} signed in for {cat_descr}, but no Agents are available.</li></h3></td></tr>";
            body += $"<tr><td valign=\"top\" ><br /><br /><br /><p style=\"font-size:16px;margin: 6px;padding: 10px;\">MCPAO - Visitor Sign In System</p></td></tr></table>";

            try
            {
                // The recipient needs to be in in table wait_time_notify for the category
                VsisMail mail = new VsisMail(
                    "no-reply@manateepao.com",
                    "'MCPAO VSIS' <no-reply@manateepao.com>",
                    notifer.Mail,
                    $"VSIS - {cat_descr} Alert",
                    "",
                    body
                );
                mail.SendTheMail();
            }
            catch (Exception)
            {
            }
        }

        public async Task GetNextInLine(string groupName, int location, string groupType)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.GetNextWaitingVisitor(groupName, location, groupType);

            if (visitor != null)
            {
                // Dispose timer and object reference, and remove visitor from Dictionary
                if (WaitedVisitors.visitorsWaiting.ContainsKey(visitor.Id))
                {
                    WaitedVisitors.visitorsWaiting[visitor.Id].WaitTimer.Dispose();
                    WaitedVisitors.visitorsWaiting[visitor.Id] = null;
                    WaitedVisitors.visitorsWaiting.Remove(visitor.Id);
                }

                await c.UpdateAgentVisitorId(groupName, visitor.Id);
                string category_description = await c.GetCategory(visitor.VisitCategoryId);
                await SendNextInLine(groupName, visitor, category_description);

                // step sequence moves to assign counter, send all counters that satisfy visitor category
                List<Counter> counters = await GetCountersByCategory(visitor.VisitCategoryId, location);
                await SendAvailableCounters(groupName, counters);

                foreach (var item in ConnectedUser.Ids)
                {
                    // send queue count to agents
                    await MessageQueueCount(item.Value, visitor.Location);
                    await MessageGroupCounterCount(item.Value, visitor.Location);
                }
            }
        }

        public async Task CancelCall(string groupName, int location, int visitorId)
        {
            ComplexProcedures c = new ComplexProcedures(_context);

            // Do not cancel until we acquire the agent name and assigned
            // counter if assigned

            // Ok to be null if call originated from counter
            Agent agent = await c.GetAgentByVisitorId(visitorId);
            
            // Will be used to send call to the assigned counter when the agent
            // originated the cancel
            Visitor visitor = await c.GetVisitorByVisitorId(visitorId);
            string assigned_counter = "";

            // hold value before call is canceled
            if (visitor != null)
                assigned_counter = visitor.AssignedCounter;

            // Do this after getting agent and visitor
            bool success = await c.CancelCallVisitor(visitorId);

            foreach (var item in ConnectedUser.Ids)
            {
                await MessageQueueCount(item.Value, visitor.Location);
                await MessageGroupCounterCount(item.Value,visitor.Location);
            }

            // send message to assigned counter
            if (assigned_counter != null)
            {
                await SendCancelCallSuccess(assigned_counter, success);
            }
            // send message to agent if exists
            if (agent != null)
            {
                await SendCancelCallSuccess(agent.AuthName, success);
            }
            // send message to caller when no assigned counter or agent
            if (assigned_counter == null && agent == null)
            {
                await SendCancelCallSuccess(groupName, success);
            }
        }
        public async Task SetAgentStatus(string groupName, string statusName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            await c.SetAgentStatus(groupName, statusName);

            foreach (var item in ConnectedUser.Ids)
            {
                await MessageGroupCounterCount(item.Value, location);
            }
        }
        public async Task SetCounterAvailableStatus(string groupName, string agentName, bool isAvailable, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            await c.SetCounterStatus(groupName, agentName, isAvailable);
            await MessageUserCounterCount(groupName, location);

            foreach (var item in ConnectedUser.Ids)
            {
                await MessageGroupCounterCount(item.Value, location);
            }
        }
        public async Task SetAgentMetrics(string groupName, int visitorId)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            await c.SaveAgentMetrics(groupName, visitorId);
        }
        public async Task GetAgentStats(string groupName, string agentName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            AgentMetric stats = await c.GetAgentMetrics(agentName);
            await SendAgentStats(groupName, stats);
        }
        public async Task SetAgentCounter(string groupName, string counter_host, int location)
        {
            if (counter_host != null)
            {
                ComplexProcedures c = new ComplexProcedures(_context);
                await FreeAgentCounter(counter_host);
                await c.SetAgentCounter(groupName, counter_host);

                foreach (var item in ConnectedUser.Ids)
                {
                    await MessageQueueCount(item.Value, location);
                    await MessageGroupCounterCount(item.Value, location);
                }
            }
        }
        public async Task FreeAgentCounter(string counter_host)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            await c.ClearAgentCounter(counter_host);
        }
        public async Task SetVisitorCalled(string groupName, int visitorId, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.SetVisitorCalled(visitorId);

            if (visitor != null)
            {
                Counter counter = await c.GetAssignedCounterHost(visitor.AssignedCounter);

                if (counter != null)
                {
                    List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);

                    foreach (var item in devices)
                    {
                        if (item.Kind == "Display")
                        {
                            await SendNotifyVisitorDisplay(item.Name, visitor.Id, counter.DisplayDescription);
                        }
                    }

                    await SendVisitorCalled(visitor.AssignedCounter, visitor.Id);

                    // send to agent to update sequence when not from counter
                    Agent agent = await c.GetAgentByVisitorId(visitor.Id);

                    if (agent != null)
                    {
                        // cancel originated from counter
                        // send to agent to update sequence
                        if (agent.AuthName != null && agent.AuthName.Length > 0)
                        {
                            if (agent.AuthName != groupName)
                            {
                                await SendVisitorCalled(agent.AuthName, visitor.Id);
                            }
                            else
                            {
                                // cancel originated from agent
                                // send to counter to update sequence
                                await SendVisitorCalled(groupName, visitor.Id);
                            }
                        }
                    }
                }
            }
        }
        public async Task SendRemoveVisitorDisplay(int location, int id)
        {
            ComplexProcedures c = new ComplexProcedures(_context);

            List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);

            foreach (var item in devices)
            {
                if (item.Kind == "Display")
                {
                    await SendNotifyRemoveVisitorDisplay(item.Name, id);
                }
            }
        }
        public async Task SetVisitorArrived(string groupName, int visitorId)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            string visitorStatus = await c.SetVisitorArrivedValue(visitorId);

            // need to send to assigned counter and agent when
            // call initiated by agent
            Agent agent = await c.GetAgentByVisitorId(visitorId);
            if(agent != null)
            {
                await SendVisitorArrived(agent.AuthName, visitorStatus);
            }

            Visitor visitor = await c.GetVisitorById(visitorId);
            if (visitor != null)
            {
                await SendRemoveVisitorDisplay(visitor.Location, visitor.Id);
                await SendVisitorArrived(visitor.AssignedCounter, visitorStatus);
            }
        }
        public async Task SetCloseCall(string groupName, int visitorId)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.GetVisitorToClose(visitorId);
            if (visitor != null)
            {
                // need to send to assigned counter and agent when
                // call initiated by agent
                // do this before closing call
                Agent agent = await c.GetAgentByVisitorId(visitorId);

                // collect call session
                await SetAgentMetrics(groupName, visitorId);

                bool isClosed = await c.CloseCall(visitor);

                if (agent != null)
                {
                    await SendCallWasClosed(agent.AuthName, isClosed);
                }

                await SendCallWasClosed(visitor.AssignedCounter, isClosed);

                if(isClosed)
                {
                    await GetAgentUser(groupName, AgentStatusTypes.All);
                }

                foreach (var item in ConnectedUser.Ids)
                {
                    await MessageQueueCount(item.Value, visitor.Location);
                    await MessageGroupCounterCount(item.Value, visitor.Location);
                }
                //await MessageQueueCount(groupName, visitor.Location);
            }
        }
        public async Task SetAssignedCounter(string groupName, int visitor_id, string counter_host)
        {
            ComplexProcedures c = new ComplexProcedures(_context);

            // store visitors taken before counter assignment for messaging
            List<Visitor> visitors = await c.GetAllVisitorsTaken();

            // get and send agent who assigned counter or null
            Agent agent = await c.GetAgentByVisitorId(visitor_id);
            //

            string assigner = "";
            if (agent != null)
            {
                if (agent.AuthName != null && agent.AuthName != "")
                    assigner = agent.AuthName;
            }

            Visitor visitor = await c.SetVisitorAssignedCounter(visitor_id, counter_host);

            if (visitor != null)
            {
                await c.SetCounterAssigned(counter_host);

                Counter counter = await c.GetAssignedCounterHost(visitor.AssignedCounter);
                int counters_count = await c.GetAvailableCountersCount(groupName, visitor.Location);

                await SendVisitorInfo(groupName, visitor);

                // notify updated available counters
                // clients shall ignore message if sequence is not AssignCounter

                List<Counter> counters = await c.GetCounters();
                List<Agent> agents = await c.GetOtherAgentsInCall(groupName);

                foreach (var v in visitors)
                {
                    foreach (var a in agents)
                        await SendAvailableCounters(a.AuthName, counters);
                }

                await AgentsByCategory(groupName, visitor.VisitCategoryId, visitor.Location);

                // send to agent
                await SendCounterAssigned(groupName, counter, counters_count, visitor.Id, assigner);

                // send to assigned counter
                await SendCounterAssigned(counter.Host, counter, counters_count, visitor.Id, assigner);
                //
            }
        }
        public async Task SendCategories(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Category> cats = await c.GetCategories(location);
            await SendCategoriesToLocation(groupName, cats);
        }
        public async Task SendQueueToDisplays(int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Queue> queue = await c.GetQueue();
            
            List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);
            
            foreach (var item in devices)
            {
                if (item.Kind == "Display")
                {
                    await SendQueueAll(item.Name, queue);
                }
            }
        }
        public async Task SendVisitorToDisplays(int location, Visitor visitor)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Queue queue = await c.GetQueueByVisitor(visitor);
            
            if (queue != null)
            {
                List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);
                foreach (var item in devices)
                {
                    if (item.Kind == "Display")
                    {
                        await SendQueueVisitor(item.Name, queue);
                    }
                }
            }
        }
        public async Task MessageQueueCount(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            int count = await c.GetQueueCountByAgent(groupName, location);
            await SendQueueCount(groupName, count);
        }
        public async Task MessageUserCounterCount(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            //List<string> agents = await GetAgents(AgentStatusTypes.Available);
            List<Counter> counters = await GetAvailableCounters();
            await SendCountersCount(groupName, counters.Count);
        }
        public async Task MessageGroupCounterCount(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);

            List<string> agents = await GetAgents(AgentStatusTypes.All);
            List<Counter> counters = await GetAvailableCounters();

            await SendCountersCount(groupName, counters.Count);

            if (counters != null)
            {
                foreach (var item in counters)
                {
                    if (item.Host != groupName)
                        await SendCountersCount(item.Host, counters.Count);
                }
            }

            if (agents != null)
            {
                foreach (var aname in agents)
                {
                    if (aname != groupName)
                    {
                        if (ConnectedUser.Ids.ContainsValue(aname))
                        {
                            await SendCountersCount(aname, counters.Count);

                            int qcount = await c.GetQueueCountByAgent(aname, location);
                            string acounter = await c.GetAgentCounter(aname);
                            if (acounter == "")
                                acounter = aname;
                            if (acounter != null && acounter != "")
                                await SendQueueCount(acounter, qcount);

                        }
                    }
                }
            }
        }
        public async Task<bool?> GetAgent(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Agent agent = await c.AgentContext(groupName);
            if (agent != null)
            {
                await SendAgentContext(groupName, agent);
                return agent.VisitorId > 0;
            }
            return false;
        }
        public async Task<bool?> GetCounterInCall(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Agent agent = await c.GetCounterFromGroupName(groupName);
            if (agent != null)
                return agent.Counter != null;

            return false;
        }
        public async Task GetAgentUser(string groupName, AgentStatusTypes statusType)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<VsisUser> agent_user = null;

            switch (statusType)
            {
                case AgentStatusTypes.All:
                    agent_user = await c.AgentUserNames(groupName);
                    break;
                case AgentStatusTypes.Available:
                    break;
                case AgentStatusTypes.Unavailable:
                    break;
            }
            
            if (agent_user != null)
            {
                await SendAgents(groupName, agent_user);
            }
        }
        public async Task GetUserContext(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            VsisUser user = await c.UserContext(groupName);
            await SendUserContext(groupName, user);
        }
        public async Task AgentsByCategory(string groupName, ulong categoryId, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<VsisUser> agents = await c.GetAgentsByCategory(categoryId, location);
            await SendAgentsInCategory(groupName, agents);
        }
        public async Task GetCounter(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Counter counter = await c.GetCounterContext(groupName);
            await SendAuthCounter(groupName, counter);
        }
        private async Task GetAppSettings(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Location> locations = await c.GetAllLocations();
            await SendOfficeLocations(groupName, locations);
            await GetAllCounters(groupName);
        }
        private async Task GetPresentCall(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.GetVisitorByAgent(groupName);
            if (visitor != null)
            {
                // get and send agent who assigned counter or null
                Agent agent = await c.GetAgentByVisitorId(visitor.Id);                
                string assigner = "";
                if(agent.AuthName != null && agent.AuthName != "")
                    assigner = agent.AuthName;

                // include call reason description
                string category_description = await c.GetCategory(visitor.VisitCategoryId);

                Counter counter = await c.GetAssignedCounterHost(visitor.AssignedCounter);
                
                int counters_count = await c.GetAvailableCountersCount(groupName, location);

                // send count of counters available for location
                await SendCounterAssigned(groupName, counter, counters_count, visitor.Id, assigner);
                
                // send to agent
                await SendPresentCallInfo(groupName, visitor, category_description, counter);

                if (counter != null)
                {
                    // send to assigned counter
                    if (groupName != visitor.AssignedCounter)
                        await SendPresentCallInfo(counter.Host, visitor, category_description, counter);
                }
            }
        }
        private async Task GetPresentCallCounter(string groupName, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.GetVisitorByCounter(groupName);
            if (visitor != null)
            {
                // get and send agent who assigned counter or null
                Agent agent = await c.GetCounterFromGroupName(groupName);
                string assigner = "";
                if (agent.AuthName != null && agent.AuthName != "")
                    assigner = agent.AuthName;

                // include call reason description
                string category_description = await c.GetCategory(visitor.VisitCategoryId);

                Counter counter = await c.GetAssignedCounterHost(visitor.AssignedCounter);

                int counters_count = await c.GetAvailableCountersCount(groupName, location);

                // send count of counters available for location
                await SendCounterAssigned(groupName, counter, counters_count, visitor.Id, assigner);

                // send to agent
                await SendPresentCallInfo(groupName, visitor, category_description, counter);
            }
        }
        public async Task GetPresentCallByVisitorId(string groupName, int location, int visitorId)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            Visitor visitor = await c.GetVisitorByVisitorId(visitorId);
            if (visitor != null)
            {
                // send call reason description
                string category_description = await c.GetCategory(visitor.VisitCategoryId);
                
                Counter counter = await c.GetAssignedCounterHost(visitor.AssignedCounter);
                
                // send to counter
                await SendPresentCallInfo(counter.Host, visitor, category_description, counter);
            }
        }
        public async Task GetAgentStatus(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            string agentStatus = await c.GetAgentStatus(groupName);
            await SendAgentStatus(groupName, agentStatus);
        }
        public async Task GetCounterStatus(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            bool tf = await c.GetCounterStatus(groupName);
            await SendCounterStatus(groupName, tf);
        }
        public async Task GetAllCounters(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Counter> counters = await c.GetAllCounters();
            await SendAllCounters(groupName, counters);
        }
        private async Task<List<Counter>> GetAvailableCounters()
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Counter> counters = await c.GetCounters();
            return counters;
        }
        private async Task<List<Counter>> GetCountersByCategory(ulong category, int location)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Counter> counters = await c.GetCountersByCategory(category, location);
            return counters;
        }
        public async Task<List<Counter>> GetAvailableCounters(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Counter> counters = await c.GetCounters();
            return counters;
        }
        public async Task GetAvailableCountersSend(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<Counter> counters = await c.GetCounters();
            await SendAvailableCounters(groupName, counters);
        }
        public async Task<List<string>> GetAgents(AgentStatusTypes statusType)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            switch (statusType)
            {
                case AgentStatusTypes.All:

                    return await c.GetAllAgents();

                case AgentStatusTypes.Available:

                    return await c.GetAvailableAgents();

                case AgentStatusTypes.Unavailable:

                    return await c.GetUnAvailableAgents();

                default:
                    return null;
            }
        }
        public async Task GetAgentNames(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            List<string> agents = await c.GetAgentNames();
            await SendAgentNames(groupName, agents);
        }       
        private async Task<bool?> CheckIfCounter(string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            return await c.IsCounter(groupName);
        }
        private async Task SetAvailableCounter(string agentName, string groupName)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            await c.SetCounterAvailable(agentName, groupName);
        }
        private async Task SendVisitorInfo(string groupName, Visitor visitor)
        {
            ComplexProcedures c = new ComplexProcedures(_context);
            string category_description = await c.GetCategory(visitor.VisitCategoryId);
                await SendVisitorInfo(groupName, visitor, category_description);
        }

        //public async Task JoinGroup(string groupName, int location, string groupType, string agentName = null)
        //{
        //    string f = "greg1";
        //    string l = "bologna1";

        //    Visitor v = new Visitor();
        //    v.Id = 4;
        //    v.AssignedCounter = "";
        //    v.Created = DateTime.Now;
        //    v.FirstName = f;
        //    v.LastName = l;
        //    v.Location = 1;
        //    v.StatusName = "WAITING";
        //    v.VisitCategoryId = 16;

        //    await AddVisitor("gbologna", v);
        //}

        public async Task JoinGroup(string groupName, int location, string groupType, string agentName = null)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                bool? hasCall = false;

                switch (groupType)
                {
                    case "Agent":
                    case "Counter":
                    case "Manager":
                        ConnectedUser.Ids.Add(Context.ConnectionId, groupName);
                        break;
                }

                switch (groupType)
                {
                    case "Kiosk":
                        // return categories
                        await SendCategories(groupName, location);
                        break;

                    case "Display":
                        // respond with queue
                        await SendQueueToDisplays(location);
                        break;

                    case "Agent":

                        await GetAppSettings(groupName);

                        hasCall = await GetAgent(groupName);
                        if (hasCall == true)
                            await GetPresentCall(groupName, location);

                        break;

                    case "Counter":

                        // set counter available when online
                        // do this before messaging queue
                        await SetAvailableCounter(agentName, groupName);
                        await GetCounter(groupName);
                        await GetAppSettings(groupName);

                        hasCall = await GetCounterInCall(groupName);
                        if (hasCall == true)
                            await GetPresentCallCounter(groupName, location);

                        break;

                    case "Manager":
                        break;
                }

                switch (groupType)
                {
                    case "Agent":
                    case "Counter":
                    case "Manager":
                        foreach (var item in ConnectedUser.Ids)
                        {
                            await MessageUserCounterCount(item.Value, location);
                            await MessageQueueCount(item.Value, location);
                        }
                        break;
                }
            }
            catch (Exception){}
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        #endregion

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUser.Ids.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    public static class ConnectedUser
    {
        public static Dictionary<string, string> Ids = new Dictionary<string, string>();
    }

    public static class WaitedVisitors
    {
        public static Dictionary<int, VisitorWaitingTimer> visitorsWaiting = new Dictionary<int, VisitorWaitingTimer>();
    }
}
