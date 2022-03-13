
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
using VisitorSignInSystem.Server.BLL;
//using System.Diagnostics;
using VisitorSignInSystem.Server.Models;
//using VisitorSignInSystem.Server.Authentication;

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
        public enum ManagerClientMessage
        {
            ManagerClientMessageQueue,
            ManagerClientMessageAgents,
            ManagerClientMessageCounters,
            ManagerClientMessageAgentMetrics,
            ManagerClientMessageCategoryMetrics,
            ManagerClientMessageVisitorMetrics
        }
        /// <summary>
        /// Transfer steps bitwise operation
        /// double last value to add new step
        /// </summary>
        [Flags]
        private enum TransferStepsCompleted
        {
            None = 0,
            Step1 = 2,
            Step2 = 4,
            Step3 = 8,
            Step4 = 16,
            Step5 = 32,
            Step6 = 64
        }

        public vsisHub(vsisdataContext context)
        {
            _context = context;
        }

        #region Send_SignalR
        //
        // Do not repeat send async string names
        //
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
        public Task SendCounterStatus(string groupName, string counterStatus)
        {
            return Clients.Group(groupName).SendAsync("CounterStatus", counterStatus);
        }
        public Task SendAllCounters(string groupName, List<Counter> counters)
        {
            return Clients.Group(groupName).SendAsync("CountersAllList", counters);
        }
        public Task SendCounters(string groupName, List<Counter> counters)
        {
            return Clients.Group(groupName).SendAsync("CountersList", counters);
        }
        public Task SendTransferReasons(string groupName, List<Transfer> reasons)
        {
            return Clients.Group(groupName).SendAsync("TransferReasons", reasons);
        }
        public Task SendAvailableCounters(string groupName, List<Counter> counters)
        {
            return Clients.Group(groupName).SendAsync("CountersAvailableList", counters);
        }
        public Task SendNoVisitorsToTake(string groupName, string msg)
        {
            return Clients.Group(groupName).SendAsync("NoVisitorsToTake", msg);
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
        private Task SendNotifyAgentsTransfer(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("NotifyAgentsTransfer", message);
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
        private Task SendTransferVisitorStatus(string groupName, bool isSuccess, string completedSteps)
        {
            return Clients.Group(groupName).SendAsync("TransferVisitorStatus", isSuccess, completedSteps);
        }
        private Task SendIsDepartmentAvailable(string groupName, bool isAvailable)
        {
            return Clients.Group(groupName).SendAsync("IsDepartmentAvailable", isAvailable);
        }
        private Task SendNotifyRemoveVisitorDisplay(string groupName, int id)
        {
            return Clients.Group(groupName).SendAsync("NotifyRemoveVisitorDisplay", id);
        }
        private Task SendAgentStats(string groupName, AgentMetric stats)
        {
            return Clients.Group(groupName).SendAsync("AgentStats", stats);
        }
        private Task SendReadyState(string groupName, string ready_state)
        {
            return Clients.Group(groupName).SendAsync("ReadyState", ready_state);
        }
        private Task SendVisitorArrived(string groupName, string visitorStatus)
        {
            return Clients.Group(groupName).SendAsync("VisitorArrived", visitorStatus);
        }
        private Task SendCallClosedStatus(string groupName, bool isClosed)
        {
            return Clients.Group(groupName).SendAsync("CallWasClosed", isClosed);
        }
        private Task SendCounterAssigned(string groupName, Counter counter, int counters_count, int visitorId, string assigner)
        {
            return Clients.Group(groupName).SendAsync("CounterAssigned", counter, counters_count, visitorId, assigner);
        }
        private Task SendVisitorQueue(string groupName, List<VisitorDetail> visitors)
        {
            return Clients.Group(groupName).SendAsync("VisitorQueueList", visitors);
        }
        public Task SendAgentsStatus(string groupName, List<AgentProfile> agents)
        {
            return Clients.Group(groupName).SendAsync("AgentsStatusList", agents);
        }
        public Task SendVisitorMetrics(string groupName, List<VisitorMetricCounts> vm)
        {
            return Clients.Group(groupName).SendAsync("VisitorMetrics", vm);
        }
        public Task SendAgentMetric(string groupName, List<AgentMetricDetail> am)
        {
            return Clients.Group(groupName).SendAsync("AgentMetric", am);
        }
        public Task SendCategoryMetrics(string groupName, List<CategoryMetric> am)
        {
            return Clients.Group(groupName).SendAsync("CategoryMetrics", am);
        }
        public Task SendAgentsStatusItemMgr(string groupName, AgentProfile agent)
        {
            return Clients.Group(groupName).SendAsync("AgentsStatusItem", agent);
        }
        public Task SendAllCountersStatusMgr(string groupName, List<CounterDetail> counters)
        {
            return Clients.Group(groupName).SendAsync("AllCountersStatus", counters);
        }
        private Task SendVisitorQueueItemMgr(string groupName, VisitorDetail visitors)
        {
            return Clients.Group(groupName).SendAsync("VisitorQueueItem", visitors);
        }
        public Task SendCounterStatusMgr(string groupName, CounterDetail counter)
        {
            return Clients.Group(groupName).SendAsync("CounterStatusMgr", counter);
        }
        public Task SendVsisUsersMgr(string groupName, List<VsisUserDetail> users)
        {
            return Clients.Group(groupName).SendAsync("VsisUsersList", users);
        }
        public Task SendDepartmentDetailList(string groupName, List<DepartmentDetail> depts)
        {
            return Clients.Group(groupName).SendAsync("DepartmentDetailList", depts);
        }
        public Task SendDepartmentList(string groupName, List<Department> depts)
        {
            return Clients.Group(groupName).SendAsync("DepartmentList", depts);
        }
        public Task SendTransferReasonsList(string groupName, List<Transfer> transfers)
        {
            return Clients.Group(groupName).SendAsync("TransferReasonsList", transfers);
        }
        public Task SendWaitTimesList(string groupName, List<WaitTimeNotify> w)
        {
            return Clients.Group(groupName).SendAsync("WaitTimeNotifyList", w);
        }

        // Currently for kiosk
        private Task SendCategoriesToLocation(string groupName, List<Category> cats)
        {
            return Clients.Group(groupName).SendAsync("Categories", cats);
        }
        // Currently for manager
        public Task SendCategoryList(string groupName, List<Category> cats)
        {
            return Clients.Group(groupName).SendAsync("CategoriesList", cats);
        }
        public Task SendIconList(string groupName, List<IconInventory> icons)
        {
            return Clients.Group(groupName).SendAsync("IconList", icons);
        }
        /// <summary>
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="send_method"></param>
        /// <param name="reason_text"></param>
        /// <param name="description_text"></param>
        /// <returns></returns>
        private Task SendDataChangeApplied(string groupName, string return_change_applied_page, string reason_text, string description_text)
        {
            return Clients.Group(groupName).SendAsync(return_change_applied_page, reason_text, description_text);
        }
        private Task SendUserNotInRole(string groupName)
        {
            return Clients.Group(groupName).SendAsync("UserNotInRole");
        }

        //public Task SendAuthenticationStatus(string groupName, bool status)
        //{
        //    return Clients.Group(groupName).SendAsync("AuthenticationStatus", status);
        //}

        #endregion Send_SignalR

        #region Receive_Messages

        /// <summary>
        /// Adds new visitor to queue
        /// Called from Kiosk
        /// TODO: check that kiosk can handle
        /// when unsuccessful.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public async Task AddVisitor(string groupName, Visitor visitor)
        {
            if (visitor != null)
            {
                ComplexProcedures c = new(_context);
                bool success = await c.SaveNewVisitor(visitor);

                // goes to kiosk
                await SendVisitorWasAdded(groupName, success);

                if (success)
                {
                    // goes to display

                    // gwb, bugfix, send full queue to force display refresh
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
                            await SendNotifyNewVisitor(auth_name, visitor.FirstName, visitor.LastName, category_descr);
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
                        VisitorWaitingTimer visitorWait = new();
                        visitorWait.StartVisitorTimer(visitor, cat_descr, notifer.MaxWaitTimeMinutes, notifer.Mail);
                        if (!WaitedVisitors.visitorsWaiting.ContainsKey(visitor.Id))
                            WaitedVisitors.visitorsWaiting.TryAdd(visitor.Id, visitorWait);
                    }

                    // Send message to manager
                    await ManagerClientMessageVisitor(visitor.Id);
                }
            }
        }

        /// <summary>
        /// Send email to manager when no agents avaialble
        /// </summary>
        /// <param name="notifer"></param>
        /// <param name="visitor"></param>
        /// <param name="cat_descr"></param>
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
                    "no-reply@xxxxxxxxx.com",
                    "'MCPAO VSIS' <no-reply@xxxxxxxxx.com>",
                    notifer.Mail,
                    $"VSIS - {cat_descr} Alert",
                    "",
                    body
                );
                mail.SendTheMail();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the next visitor in queue
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public async Task GetNextInLine(string groupName, int location, string groupType)
        {
            ComplexProcedures c = new(_context);
            Visitor visitor = await c.GetNextWaitingVisitor(groupName, location, groupType);

            if (visitor != null)
            {
                // Dispose timer and object reference, and remove visitor from Dictionary
                if (WaitedVisitors.visitorsWaiting.ContainsKey(visitor.Id))
                {
                    WaitedVisitors.visitorsWaiting[visitor.Id].WaitTimer.Dispose();
                    WaitedVisitors.visitorsWaiting[visitor.Id] = null;

                    VisitorWaitingTimer removedItem;
                    bool result = WaitedVisitors.visitorsWaiting.TryRemove(visitor.Id, out removedItem);
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

                // Send messages to manager
                await ManagerClientMessageVisitor(visitor.Id);

            }
            else
            {
                // Message when visitor is null otherwise agent will advance to next step
                await SendNoVisitorsToTake(groupName, "null");
            }
        }

        /// <summary>
        /// Cancels a visitor taken by agent. 
        /// Available up to but not including called status.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task CancelCall(string groupName, int location, int visitorId)
        {
            ComplexProcedures c = new(_context);

            // Do not cancel until we acquire the agent name and assigned
            // counter if assigned

            // Ok to be null if call originated from counter
            Agent agent = await c.GetAgentByVisitorId(visitorId);

            // Will be used to send call to the assigned counter when the agent
            // originated the cancel
            //Visitor visitor = await c.GetVisitorByVisitorId(visitorId);

            // Do this after getting agent and visitor
            bool success = await c.CancelCallVisitor(visitorId);
            if (success)
                agent = null;

            foreach (var item in ConnectedUser.Ids)
            {
                await MessageQueueCount(item.Value, location);
                await MessageGroupCounterCount(item.Value, location);
            }

            // send message to assigned counter
            // default message to caller when no assigned counter or agent
            string message_to = groupName;

            // send message to agent if exists
            if (agent != null)
                message_to = agent.AuthName;

            // Send message to manager
            if (success)
                await ManagerClientMessageVisitor(visitorId);

            await SendCancelCallSuccess(message_to, success);
        }

        /// <summary>
        /// Check if a department has an agent available.
        /// Required when doing a transfer to department
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public async Task IsDepartmentAvailable(string groupName, sbyte department)
        {
            ComplexProcedures c = new(_context);
            bool tf = await c.IsDepartmentAvailable(department);
            await SendIsDepartmentAvailable(groupName, tf);
        }

        /// <summary>
        /// Transfers a visitor taken by agent.
        /// All steps shall succeed.
        /// TODO: should this be a stored procedure transaction instead?
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task TransferVisitor(string groupName, ulong categoryId, int visitorId)
        {
            ComplexProcedures c = new(_context);

            bool success = false;
            TransferStepsCompleted steps = TransferStepsCompleted.None;

            // visitor is required for steps 2 and 4
            // do not proceed if visitor is null (unlikely since this is a visitor transfer)
            Visitor visitor = await c.GetVisitorByVisitorId(visitorId);

            if (visitor != null)
            {
                // *********************************************************
                // Step 1. Clear visitor on agent,
                // make agent available and save agent metrics
                // *********************************************************
                success = await c.CloseAgentVisitor(visitorId);
                if (success)
                {
                    steps = steps | TransferStepsCompleted.Step1;

                    success = await SendCloseCallMessages(visitor, groupName, true);
                    if (success)
                        steps = steps | TransferStepsCompleted.Step2;
                }


                // *********************************************************
                // Step 2. Change visitor reason
                // Change visitor status to Waiting
                // *********************************************************
                success = await c.ChangeVisitorReason(categoryId, visitorId);
                if (success)
                    steps = steps | TransferStepsCompleted.Step3;


                // do not clear assigned counter

                // *********************************************************
                // Step 3. Make assigned counter available
                // Clear visitor assigned counter on object
                // *********************************************************
                if (visitor != null)
                {
                    success = await c.SetCounterAvailable(visitor.AssignedCounter);
                    if (success)
                    {
                        visitor.AssignedCounter = null;
                        steps = steps | TransferStepsCompleted.Step4;
                    }
                }


                // *********************************************************
                // Step 4. Change visitor status and put back in queue
                // position in queue will remain at sign in time
                // visitor will not show on display
                // *********************************************************
                success = await c.FlagTransferVisitor(visitorId, true);
                if (success)
                    steps = steps | TransferStepsCompleted.Step5;


                // *********************************************************
                // Step 5. Add transfer record
                // *********************************************************
                success = await c.SaveTransferRecord(visitor.Id);
                if (success)
                    steps = steps | TransferStepsCompleted.Step6;

            } // end steps


            // determine the successful steps
            TransferStepsCompleted flagValue = TransferStepsCompleted.Step1 |
                                                TransferStepsCompleted.Step2 |
                                                TransferStepsCompleted.Step3 |
                                                TransferStepsCompleted.Step4 |
                                                TransferStepsCompleted.Step5 |
                                                TransferStepsCompleted.Step6;

            // make sure all the steps were set
            success = steps.HasFlag(flagValue);

            // callback
            await SendTransferVisitorStatus(groupName, success, steps.ToString());

            // message dept.
            if (success)
            {
                await SendTransferredVisitorToDisplays(visitor.Location, visitor);

                string category_description = await c.GetCategory(categoryId);

                string message = $"Visitor { visitor.FirstName} { visitor.LastName} was transferred to '{category_description}'.";

                //
                // Send message to connected agents in transferred category
                //
                foreach (var item in ConnectedUser.Ids)
                {
                    string auth_name = item.Value;

                    // change auth_name if agent is at counter
                    Agent agent = await c.GetCounterFromGroupName(item.Value);
                    if (agent != null)
                        auth_name = agent.AuthName;

                    // Get agent auth name
                    string agent_name = await c.GetAgentByCategory(auth_name, categoryId, visitor.Location);

                    // Get department for this category
                    var dept = await c.LookupDepartmentId(categoryId);

                    // Send toast to agents in transferred department

                    if (agent_name != null && agent_name.Length > 0)
                    {
                        VsisUser user = await c.UserContext(agent_name);

                        if (dept == user.Department)
                        {
                            await SendNotifyAgentsTransfer(item.Value, message);
                            await MessageUserCounterCount(item.Value, visitor.Location);
                            await MessageQueueCount(item.Value, visitor.Location);
                        }
                    }
                }
                //
                // Send message to manager
                //
                await ManagerClientMessageVisitor(visitor.Id);
            }
        }

        /// <summary>
        /// Sent from agent client
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="statusName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SetAgentStatus(string groupName, string statusName, int location)
        {
            try
            {
                ComplexProcedures c = new(_context);
                await c.SetAgentStatus(groupName, statusName);

                // return agent status to caller
                await GetAgentStatus(groupName);

                // Send new counter count to all connected users
                foreach (var item in ConnectedUser.Ids)
                {
                    await MessageGroupCounterCount(item.Value, location);
                }

                // Send message to manager
                await ManagerClientMessageAgent(groupName);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Called by agent clients
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="agentName"></param>
        /// <param name="isAvailable"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SetCounterAvailableStatus(string groupName, string agentName, bool isAvailable, int location)
        {
            ComplexProcedures c = new(_context);
            await c.SetCounterStatus(groupName, agentName, isAvailable);

            await GetCounterStatus(groupName);

            await MessageUserCounterCount(groupName, location);

            foreach (var item in ConnectedUser.Ids)
            {
                await MessageGroupCounterCount(item.Value, location);
            }

            // Send message to manager
            await ManagerClientMessageCounter(groupName);
            await ManagerClientMessageAgent(agentName);

        }

        /// <summary>
        /// Save the time agent spent with visitor
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task SetAgentMetrics(string groupName, int visitorId)
        {
            ComplexProcedures c = new(_context);
            await c.SaveAgentMetrics(groupName, visitorId);
        }

        /// <summary>
        /// Get the agent stats
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAgentStats(string groupName)
        {
            ComplexProcedures c = new(_context);
            AgentMetric stats = await c.GetAgentMetrics(groupName);
            await SendAgentStats(groupName, stats);
        }

        /// <summary>
        /// Called from client
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="counter_host"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SetAgentCounter(string groupName, string counter_host, int location)
        {
            try
            {
                if (counter_host != null)
                {
                    ComplexProcedures c = new(_context);
                    await FreeAgentCounter(counter_host);
                    await c.SetAgentCounterHost(groupName, counter_host);
                    await GetAgentStats(groupName);

                    foreach (var item in ConnectedUser.Ids)
                    {
                        await MessageQueueCount(item.Value, location);
                        await MessageGroupCounterCount(item.Value, location);
                    }

                    // Send message to manager
                    await ManagerClientMessageCounter(counter_host);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Called from agent when changing counters
        /// </summary>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task FreeAgentCounter(string counter_host)
        {
            ComplexProcedures c = new(_context);
            await c.ClearAgentCounter(counter_host);
        }

        /// <summary>
        /// Update the visitor called status
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SetVisitorCalled(string groupName, int visitorId, int location)
        {
            ComplexProcedures c = new(_context);
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
                // Send message to manager
                await ManagerClientMessageVisitor(visitor.Id);
            }
        }


        /// <summary>
        /// Send messages to connected managers
        /// ManagerClientMessages type
        /// Do not use in loops
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        private async Task ManagerClientMessages(ManagerClientMessage mgr)
        {
            if (ConnectedManagers.Ids.Count > 0)
            {
                ComplexProcedures c = new(_context);
                string[] roles = { "Manager" };

                switch (mgr)
                {
                    case ManagerClientMessage.ManagerClientMessageQueue:

                        // Send updated visitor queue to manager client

                        List<VisitorDetail> visitors = await c.GetAllVisitors();

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendVisitorQueue(item.Value, visitors);
                        }
                        break;

                    case ManagerClientMessage.ManagerClientMessageCounters:

                        // Send updated counter status to manager client

                        List<CounterDetail> counters = await c.GetAllCountersStatus();

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendAllCountersStatusMgr(item.Value, counters);
                        }
                        break;

                    case ManagerClientMessage.ManagerClientMessageAgents:

                        // Send updated agent status to manager client
                        List<AgentProfile> agents = await c.GetAgentStatusList(ConnectedUser.Ids.ToList());

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendAgentsStatus(item.Value, agents);
                        }
                        break;

                    case ManagerClientMessage.ManagerClientMessageVisitorMetrics:

                        List<VisitorMetricCounts> vm = await c.GetVisitorMetricsByCategory();

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendVisitorMetrics(item.Value, vm);
                        }
                        break;

                    case ManagerClientMessage.ManagerClientMessageAgentMetrics:

                        List<AgentMetricDetail> am = await c.GetAllAgentMetrics();

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendAgentMetric(item.Value, am);
                        }
                        break;

                    case ManagerClientMessage.ManagerClientMessageCategoryMetrics:

                        List<CategoryMetric> cat = await c.GetCategoryMetrics();

                        foreach (var item in ConnectedUser.Ids)
                        {
                            VsisUser user = await c.UserContext(item.Value);
                            if (user != null && roles.Contains(user.Role))
                                await SendCategoryMetrics(item.Value, cat);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// ManagerClientMessageVisitor
        /// Send a single visitor to manager
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        private async Task ManagerClientMessageVisitor(int visitorId)
        {
            try
            {
                if (ConnectedManagers.Ids.Count > 0)
                {
                    ComplexProcedures c = new(_context);
                    string[] roles = { "Manager" };

                    // Send updated visitor to manager client

                    List<VisitorDetail> visitor = await c.GetOneVisitor(visitorId);

                    if (!(visitor.Count > 0))
                    {
                        // set visitor to be removed in manager grid
                        visitor = new List<VisitorDetail>();
                        visitor.Add(new VisitorDetail { Id = visitorId, StatusName = "CLOSED" });
                    }

                    foreach (var item in ConnectedUser.Ids)
                    {
                        VsisUser user = await c.UserContext(item.Value);
                        if (user != null && roles.Contains(user.Role))
                            await SendVisitorQueueItemMgr(item.Value, visitor[0]);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ManagerClientMessageAgent
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        private async Task ManagerClientMessageAgent(string authName)
        {
            try
            {
                if (ConnectedManagers.Ids.Count > 0)
                {
                    ComplexProcedures c = new(_context);
                    string[] roles = { "Manager" };

                    // Send updated visitor to manager client

                    AgentProfile agent = await c.GetAgentStatusMgr(authName);

                    foreach (var item in ConnectedUser.Ids)
                    {
                        VsisUser user = await c.UserContext(item.Value);
                        if (user != null && roles.Contains(user.Role))
                            await SendAgentsStatusItemMgr(item.Value, agent);
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        private async Task ManagerClientMessageCounter(string host)
        {
            try
            {
                if (ConnectedManagers.Ids.Count > 0)
                {
                    ComplexProcedures c = new(_context);
                    string[] roles = { "Manager" };

                    CounterDetail counter = c.GetCountersStatus(host).FirstOrDefault();

                    foreach (var item in ConnectedUser.Ids)
                    {
                        VsisUser user = await c.UserContext(item.Value);
                        if (user != null && roles.Contains(user.Role))
                            await SendCounterStatusMgr(item.Value, counter);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Display operation to remove a visitor when arrived
        /// </summary>
        /// <param name="location"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SendRemoveVisitorDisplay(int location, int id)
        {
            ComplexProcedures c = new(_context);

            List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);

            foreach (var item in devices)
            {
                if (item.Kind == "Display")
                {
                    await SendNotifyRemoveVisitorDisplay(item.Name, id);
                }
            }
        }

        /// <summary>
        /// Update visitor status to arrived
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task SetVisitorArrived(string groupName, int visitorId)
        {
            try
            {
                ComplexProcedures c = new(_context);
                string visitorStatus = await c.SetVisitorArrivedValue(visitorId);

                // need to send to assigned counter and agent when
                // call initiated by agent
                Agent agent = await c.GetAgentByVisitorId(visitorId);
                if (agent != null)
                {
                    await SendVisitorArrived(agent.AuthName, visitorStatus);
                }

                Visitor visitor = await c.GetVisitorById(visitorId);
                if (visitor != null)
                {
                    await SendRemoveVisitorDisplay(visitor.Location, visitor.Id);
                    await SendVisitorArrived(visitor.AssignedCounter, visitorStatus);

                    // Send message to manager
                    await ManagerClientMessageVisitor(visitor.Id);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Close visit and send closed call messages
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task SetCloseCall(string groupName, int visitorId)
        {
            try
            {
                ComplexProcedures c = new(_context);
                Visitor visitor = await c.GetVisitorByVisitorId(visitorId);
                if (visitor != null)
                {
                    // Send message to assigned counter and agent when call initiated by agent
                    // Get agent data object before closing call
                    Agent agent = await c.GetAgentByVisitorId(visitorId);

                    // Save agent call session data for bi
                    await SetAgentMetrics(groupName, visitorId);

                    // Close the call
                    bool isClosed = await c.CloseCall(visitor);

                    // Send message to agent
                    if (agent != null)
                    {
                        // add stats and return new values
                        await c.AddAgentStats(agent.AuthName);

                        // add category stats to category_metrics table
                        // used in manager app
                        await c.AddCategoryStats(visitor.VisitCategoryId);

                        await GetAgentStats(groupName);

                        // send closed call status
                        await SendCallClosedStatus(agent.AuthName, isClosed);
                    }

                    // Send all other close call messages
                    await SendCloseCallMessages(visitor, groupName, isClosed);

                    // Send message to manager
                    await ManagerClientMessageVisitor(visitor.Id);

                    // Send updated metrics to manager
                    await ManagerClientMessageMetrics();
                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Send message to all connected clients
        /// that the visit was closed.
        /// Used on end call and transfers.
        /// There is no guarantee that SignalR will
        /// deliver the message.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="visitor"></param>
        /// <param name="groupName"></param>
        /// <param name="isClosed"></param>
        /// <returns>bool</returns>
        public async Task<bool> SendCloseCallMessages(Visitor visitor, string groupName, bool isClosed)
        {
            try
            {
                // Send message to counter
                await SendCallClosedStatus(visitor.AssignedCounter, isClosed);

                // Send updated agent list
                if (isClosed)
                    await GetAgentUser(groupName, AgentStatusTypes.All);

                // Send message to all connected users
                foreach (var item in ConnectedUser.Ids)
                {
                    await MessageQueueCount(item.Value, visitor.Location);
                    await MessageGroupCounterCount(item.Value, visitor.Location);
                }

                return true;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Update visitor to assigned counter
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitor_id"></param>
        /// <param name="counter_host"></param>
        /// <returns></returns>
        public async Task SetAssignedCounter(string groupName, int visitor_id, string counter_host)
        {
            ComplexProcedures c = new(_context);

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

                // send to assigned counter if not the same
                if (counter.Host != null && counter.Host != groupName)
                    await SendCounterAssigned(counter.Host, counter, counters_count, visitor.Id, assigner);

                // Send message to manager
                await ManagerClientMessageVisitor(visitor.Id);
                await ManagerClientMessageCounter(counter.Host);

            }
        }

        /// <summary>
        /// Get visitor reasons types
        /// Sends category list to kiosk
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SendCategories(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            List<Category> cats = await c.GetCategories(location);
            await SendCategoriesToLocation(groupName, cats);
        }

        /// <summary>
        /// Use this to get visitors in queue.
        /// List will be sent to all display devices.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task SendQueueToDisplays(int location)
        {
            ComplexProcedures c = new(_context);
            List<Queue> queue = await c.GetQueueForDisplay(location);

            List<GroupDevice> devices = await c.GetGroupDiplaysByLocation(location);

            foreach (var item in devices)
            {
                if (item.Kind == "Display")
                    await SendQueueAll(item.Name, queue);
            }
        }

        /// <summary>
        /// Called by display to verify queue count matches 
        /// count on display
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="countReceived"></param>
        /// <returns></returns>
        public async Task VerifyQueueCount(string groupName, int location, int countReceived)
        {
            ComplexProcedures c = new(_context);
            List<Queue> queue = await c.GetQueueForDisplay(location);

            if (countReceived != queue.Count)
                await SendQueueAll(groupName, queue);
        }

        /// <summary>
        /// Send newly added visitor to display
        /// </summary>
        /// <param name="location"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public async Task SendVisitorToDisplays(int location, Visitor visitor)
        {
            ComplexProcedures c = new(_context);
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

        /// <summary>
        /// Send the transferred visitor to display
        /// with assigned counter description
        /// </summary>
        /// <param name="location"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public async Task SendTransferredVisitorToDisplays(int location, Visitor visitor)
        {
            ComplexProcedures c = new(_context);
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

        /// <summary>
        /// Initial request from manager app for visitor and category metrics
        /// </summary>
        /// <returns></returns>
        public async Task GetVisitorMetrics()
        {
            await ManagerClientMessageMetrics();
        }


        private async Task ManagerClientMessageMetrics()
        {
            //await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageVisitorMetrics);
            await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageAgentMetrics);
            await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageCategoryMetrics);
        }

        /// <summary>
        /// Send visitor queue to all connected users
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task MessageQueueCount(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            int count = await c.GetQueueCountByAgent(groupName, location);

            //
            // Send visitor queue to agent clients
            //
            await SendQueueCount(groupName, count);
        }

        /// <summary>
        /// Send counter count message to all connected clients
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task MessageUserCounterCount(string groupName, int location)
        {
            ComplexProcedures c = new(_context);

            List<Counter> counters = await GetAvailableCounters();

            //
            // Send counters list to agents clients
            //
            await SendCountersCount(groupName, counters.Count);
        }

        /// <summary>
        /// Send a message to connected clients.
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task MessageGroupCounterCount(string groupName, int location)
        {
            ComplexProcedures c = new(_context);

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
                        if (ConnectedUser.Ids.ContainsKey(aname))
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

        /// <summary>
        /// Get agent type
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool?> GetAgent(string groupName)
        {
            ComplexProcedures c = new(_context);
            Agent agent = await c.AgentContext(groupName);
            if (agent != null)
            {
                await SendAgentContext(groupName, agent);
                return agent.VisitorId > 0;
            }
            return false;
        }

        /// <summary>
        /// Get busy counters
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool?> GetCounterInCall(string groupName)
        {
            ComplexProcedures c = new(_context);
            Agent agent = await c.GetCounterFromGroupName(groupName);
            if (agent != null)
                return agent.Counter != null;

            return false;
        }

        /// <summary>
        /// Get agent user type
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public async Task GetAgentUser(string groupName, AgentStatusTypes statusType)
        {
            ComplexProcedures c = new(_context);
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
                await SendAgents(groupName, agent_user);

        }

        /// <summary>
        /// Get user type
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetUserContext(string groupName)
        {
            ComplexProcedures c = new(_context);
            VsisUser user = await c.UserContext(groupName);
            await SendUserContext(groupName, user);
        }

        /// <summary>
        /// Get any agents by category
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="categoryId"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task AgentsByCategory(string groupName, ulong categoryId, int location)
        {
            ComplexProcedures c = new(_context);
            List<VsisUser> agents = await c.GetAgentsByCategory(categoryId, location);
            await SendAgentsInCategory(groupName, agents);
        }

        /// <summary>
        /// Send visitor queue to manager
        /// Currently called by manager only
        /// Add manager role if needed
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAllVisitorsMgr(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<VisitorDetail> visitors = await c.GetAllVisitors();
            await SendVisitorQueue(groupName, visitors);
        }

        public async Task GetUsers(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<VsisUserDetail> users = await c.Users();
            await SendVsisUsersMgr(groupName, users);
        }

        public async Task GetIconList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<IconInventory> icons = await c.GetIconsList();
            await SendIconList(groupName, icons);
        }

        public async Task GetDepartmentDetailList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<DepartmentDetail> depts = await c.DepartmentDetailList();
            await SendDepartmentDetailList(groupName, depts);
        }
        public async Task GetDepartmentList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Department> depts = await c.DepartmentList();
            await SendDepartmentList(groupName, depts);
        }
        public async Task AddDepartment(string groupName, string returnChangeAppliedPage, Department dept)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddDepartment(dept);
            if (m == "success")
                await GetDepartmentList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, dept.Id.ToString());
        }
        public async Task UpdateDepartment(string groupName, string returnChangeAppliedPage, Department dept)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateDepartment(dept);
            if (m == "success")
                await GetDepartmentList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, dept.Id.ToString());
        }
        public async Task DeleteDepartment(string groupName, string returnChangeAppliedPage, int id)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteDepartment(id);
            if (m == "success")
                await GetDepartmentList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, id.ToString());
        }

        /// <summary>
        /// Use only with manager app
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetTransferTypesForManager(string groupName, string returnChangeAppliedPage)
        {
            ComplexProcedures c = new(_context);
            List<Transfer> reasons = await c.TransferTypesForManager();
            await SendTransferTypesForManager(groupName, reasons);
        }
        /// <summary>
        /// User transfer reason methods intended for manager only
        /// Calling a transfer reason method used by Agent is not permitted
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public async Task AddTransferType(string groupName, string returnChangeAppliedPage, Transfer transfer)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddTransferType(transfer);
            if (m == "success")
                await GetTransferTypesForManager(groupName, returnChangeAppliedPage);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, transfer.Description.ToString());
        }
        public Task SendTransferTypesForManager(string groupName, List<Transfer> reasons)
        {
            return Clients.Group(groupName).SendAsync("TransferTypesForManager", reasons);
        }
        public async Task UpdateTransferType(string groupName, string returnChangeAppliedPage, Transfer transfer)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateTransferType(transfer);
            if (m == "success")
                await GetTransferTypesForManager(groupName, returnChangeAppliedPage);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, transfer.Description.ToString());
        }
        public async Task DeleteTransferType(string groupName, string returnChangeAppliedPage, ulong id)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteTransferType(id);
            if (m == "success")
                await GetTransferTypesForManager(groupName, returnChangeAppliedPage);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, id.ToString());
        }

        public async Task AddUser(string groupName, string returnChangeAppliedPage, VsisUserDetail user)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddUser(user);
            if (m == "success")
                await GetUsers(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, user.AuthName);
        }
        public async Task UpdateUser(string groupName, string returnChangeAppliedPage, VsisUserDetail user)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateUser(user);
            if (m == "success")
                await GetUsers(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, user.AuthName);
        }
        public async Task DeleteUser(string groupName, string returnChangeAppliedPage, string authName)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteUser(authName);
            if (m == "success")
                await GetUsers(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, authName);
        }

        public async Task GetWaitTimesList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<WaitTimeNotify> w = await c.WaitTimes();
            await SendWaitTimesList(groupName, w);
        }
        public async Task AddWaitTime(string groupName, string returnChangeAppliedPage, WaitTimeNotify w)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddWaitTimeNotify(w);
            if (m == "success")
                await GetWaitTimesList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, w.Category.ToString());
        }
        public async Task UpdateWaitTime(string groupName, string returnChangeAppliedPage, WaitTimeNotify w)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateWaitTimeNotify(w);
            if (m == "success")
                await GetWaitTimesList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, w.Category.ToString());
        }

        public async Task DeleteWaitTime(string groupName, string returnChangeAppliedPage, ulong waitcategory, string mail)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteWaitTimeNotify(waitcategory);
            if (m == "success")
                await GetWaitTimesList(groupName);
            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, mail);
        }

        public async Task UpdateVisitorCategory(string groupName, string returnChangeAppliedPage, int visitorId, ulong visitCategoryId)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateVisitorCategoryId(visitorId, visitCategoryId);
            if (m == "success")
                await GetVisitorQueueMgr(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, visitorId.ToString());
        }

        /// <summary>
        /// Purge Visitor in Waiting status only.
        /// Currently called from manager.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task PurgeVisitorInQueue(string groupName, string returnChangeAppliedPage, int visitorId, int location)
        {
            ComplexProcedures c = new(_context);
            string m = await c.PurgeVisitorInQueue(visitorId);
            if (m == "success")
            {
                await SendQueueToDisplays(location);
                await GetVisitorQueueMgr(groupName);
            }
            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, visitorId.ToString());
        }

        public async Task GetVisitorQueueMgr(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<VisitorDetail> visitors = await c.GetAllVisitors();
            await SendVisitorQueue(groupName, visitors);
        }

        /// <summary>
        /// Send ordered categories by location
        /// Location shall support the category
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task GetCategoryList(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            List<Category> cats = await c.GetCategories(location);
            await SendCategoryList(groupName, cats);
        }
        public async Task GetAnyCategoriesList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Category> cats = await c.GetAnyCategories();
            await SendCategoryList(groupName, cats);
        }
        public async Task AddCategory(string groupName, string returnChangeAppliedPage, Category category)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddCategory(category);
            if (m == "success")
                await GetAnyCategoriesList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, category.Description);
        }
        public async Task UpdateCategory(string groupName, string returnChangeAppliedPage, Category category)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateCategory(category);
            if (m == "success")
                await GetAnyCategoriesList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, category.Description);
        }
        public async Task DeleteCategory(string groupName, string returnChangeAppliedPage, ulong id)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteCategory(id);
            if (m == "success")
                await GetAnyCategoriesList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, id.ToString());
        }

        public async Task AddCounter(string groupName, string returnChangeAppliedPage, Counter counter)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddCounter(counter);
            if (m == "success")
                await GetCounterList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, counter.Description);
        }
        public async Task UpdateCounter(string groupName, string returnChangeAppliedPage, Counter counter)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateCounter(counter);
            if (m == "success")
                await GetCounterList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, counter.Description);
        }
        public async Task DeleteCounter(string groupName, string returnChangeAppliedPage, string host)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteCounter(host);
            if (m == "success")
                await GetCounterList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, host);
        }

        private Task SendGroupDevices(string groupName, List<GroupDevice> devices)
        {
            return Clients.Group(groupName).SendAsync("GroupDeviceList", devices);
        }
        public async Task GetGroupDeviceList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<GroupDevice> devices = await c.GroupDevices();
            await SendGroupDevices(groupName, devices);
        }
        public async Task AddGroupDevice(string groupName, string returnChangeAppliedPage, GroupDevice device)
        {
            ComplexProcedures c = new(_context);
            string m = await c.AddGroupDevice(device);
            if (m == "success")
                await GetGroupDeviceList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, device.Name);
        }
        public async Task UpdateGroupDevice(string groupName, string returnChangeAppliedPage, GroupDevice device)
        {
            ComplexProcedures c = new(_context);
            string m = await c.UpdateGroupDevice(device);
            if (m == "success")
                await GetGroupDeviceList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, device.Name);
        }
        public async Task DeleteGroupDevice(string groupName, string returnChangeAppliedPage, int id)
        {
            ComplexProcedures c = new(_context);
            string m = await c.DeleteGroupDevice(id);
            if (m == "success")
                await GetGroupDeviceList(groupName);

            await SendDataChangeApplied(groupName, returnChangeAppliedPage, m, id.ToString());
        }

        /// <summary>
        /// Get counters
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetCounter(string groupName)
        {
            ComplexProcedures c = new(_context);
            Counter counter = await c.GetCounterContext(groupName);
            await SendAuthCounter(groupName, counter);
        }

        /// <summary>
        /// General settings for agent client
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task GetAppSettings(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            List<Location> locations = await c.GetAllLocations();
            await SendOfficeLocations(groupName, locations);
            await GetAllCounters(groupName);
            await GetTransferReasons(groupName, location);
        }

        /// <summary>
        /// Get an existing call to continue.
        /// Called when agent client restarts or refreshed client
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task GetPresentCall(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            Visitor visitor = await c.GetVisitorByAgent(groupName);
            if (visitor != null)
            {
                // get and send agent who assigned counter or null
                Agent agent = await c.GetAgentByVisitorId(visitor.Id);
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

                if (counter != null)
                {
                    // send to assigned counter
                    if (groupName != visitor.AssignedCounter)
                        await SendPresentCallInfo(counter.Host, visitor, category_description, counter);
                }
            }
        }

        /// <summary>
        /// Get an existing call at counter
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task GetPresentCallCounter(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
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

        /// <summary>
        /// Get an existing call by visitor
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public async Task GetPresentCallByVisitorId(string groupName, int location, int visitorId)
        {
            ComplexProcedures c = new(_context);
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

        /// <summary>
        /// Get agent status
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAgentStatus(string groupName)
        {
            ComplexProcedures c = new(_context);
            string agentStatus = await c.GetAgentStatus(groupName);
            await SendAgentStatus(groupName, agentStatus);
        }

        /// <summary>
        /// Gets counter status
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetCounterStatus(string groupName)
        {
            ComplexProcedures c = new(_context);
            bool tf = await c.GetCounterStatus(groupName);
            string status = tf ? "AVAILABLE" : "UNAVAILABLE";
            await SendCounterStatus(groupName, status);
        }

        /// <summary>
        /// Get all counters
        /// User in Agent
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAllCounters(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetAllCounters();
            await SendAllCounters(groupName, counters);
        }

        /// <summary>
        /// Get all counters
        /// Use in manager
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetCounterList(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetAllCounters();
            await SendCounters(groupName, counters);
        }

        /// <summary>
        /// Get transfer category reasons
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task GetTransferReasons(string groupName, int location)
        {
            ComplexProcedures c = new(_context);
            List<Transfer> reasons = await c.GetTransferReasons(location);
            await SendTransferReasons(groupName, reasons);
        }

        /// <summary>
        /// Get available counters
        /// </summary>
        /// <returns></returns>
        private async Task<List<Counter>> GetAvailableCounters()
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetCounters();
            return counters;
        }

        /// <summary>
        /// Get available counters by groupName
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<List<Counter>> GetAvailableCounters(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetCounters();
            return counters;
        }

        /// <summary>
        /// Get counters by category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<List<Counter>> GetCountersByCategory(ulong category, int location)
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetCountersByCategory(category, location);
            return counters;
        }

        /// <summary>
        /// Get and send available counters by groupName
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAvailableCountersSend(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<Counter> counters = await c.GetCounters();
            await SendAvailableCounters(groupName, counters);
        }

        /// <summary>
        /// Get agents by status type
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAgents(AgentStatusTypes statusType)
        {
            ComplexProcedures c = new(_context);
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

        /// <summary>
        /// Get agent names
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task GetAgentNames(string groupName)
        {
            ComplexProcedures c = new(_context);
            List<string> agents = await c.GetAgentNames();
            await SendAgentNames(groupName, agents);
        }

        /// <summary>
        /// Check if groupName is a counter
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task<bool?> CheckIfCounter(string groupName)
        {
            ComplexProcedures c = new(_context);
            return await c.IsCounter(groupName);
        }

        /// <summary>
        /// Save the agent to a counter
        /// </summary>
        /// <param name="agentName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task SetAssignAgentToCounter(string agentName, string groupName)
        {
            ComplexProcedures c = new(_context);
            await c.AssignAgentToCounter(agentName, groupName);
        }

        /// <summary>
        /// Send visitor data
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="visitor"></param>
        /// <returns></returns>
        private async Task SendVisitorInfo(string groupName, Visitor visitor)
        {
            ComplexProcedures c = new(_context);
            string category_description = await c.GetCategory(visitor.VisitCategoryId);
            await SendVisitorInfo(groupName, visitor, category_description);
        }

        /// <summary>
        /// Record User app version to users table
        /// Called from app
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task RecordUserAppVersion(string groupName, string appVersion)
        {
            ComplexProcedures c = new(_context);
            await c.RecordUserAppVersion(groupName, appVersion);
        }


        #endregion Receive_Messages

        #region ManagerTasks

        /// <summary>
        /// Send all message types to manager clients
        /// </summary>
        /// <returns></returns>
        public async Task SendAllManagerMessages()
        {
            // send visitor queue
            await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageQueue);
            // send counter list
            await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageCounters);
            // send agents
            await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageAgents);
        }

        #endregion ManagerTasks

        ///// <summary>
        ///// Authenticate a password hash
        ///// TODO: put password in configuration file
        ///// and use a lookup Id.
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <param name="hash"></param>
        ///// <returns></returns>
        //public async Task SettingsAuthentication(string groupName, byte[] hash)
        //{
        //    try
        //    {
        //        // put password in config
        //        string p = "columbia123";

        //        bool tf = false;

        //        PaoSecurity aes = new PaoSecurity();

        //        string r = aes.DecryptString(hash);
        //        if (p == r)
        //            tf = true;

        //        await SendAuthenticationStatus(groupName, tf);
        //    }
        //    catch (Exception ex) { 

        //    }
        //}

        /// <summary>
        /// The groupType refers to the application and not the user
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="location"></param>
        /// <param name="groupType"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
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
                        ConnectedUser.Ids.TryAdd(Context.ConnectionId, groupName);
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

                        await GetAppSettings(groupName, location);

                        hasCall = await GetAgent(groupName);
                        if (hasCall == true)
                            await GetPresentCall(groupName, location);

                        await GetAgentStats(groupName);
                        await SendReadyState(groupName, "ready");

                        break;

                    case "Counter":

                        // set counter available when online
                        // do this before messaging queue
                        await SetAssignAgentToCounter(agentName, groupName);
                        await GetCounter(groupName);
                        await GetAppSettings(groupName, location);
                        await GetAgentStats(groupName);

                        hasCall = await GetCounterInCall(groupName);
                        if (hasCall == true)
                            await GetPresentCallCounter(groupName, location);

                        await SendReadyState(groupName, "ready");

                        // send counter list to manager
                        await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageCounters);

                        break;

                    case "Manager":

                        bool tf = await GetUserRole(groupName);
                        if (tf)
                        {
                            ConnectedManagers.Ids.TryAdd(Context.ConnectionId, groupName);

                            // Send categories
                            await GetAnyCategoriesList(groupName);

                            // Send all manager messages for all locations
                            await SendAllManagerMessages();
                        }
                        else
                        {
                            await SendUserNotInRole(groupName);
                        }
                        break;
                }

                // Messages are intended for all connected users
                switch (groupType)
                {
                    case "Agent":
                    case "Counter":
                        foreach (var item in ConnectedUser.Ids)
                        {
                            await MessageUserCounterCount(item.Value, location);
                            await MessageQueueCount(item.Value, location);
                        }
                        // send agent list to manager
                        await ManagerClientMessages(ManagerClientMessage.ManagerClientMessageAgents);
                        break;
                }
            }
            catch (Exception)
            {
                // Debug.WriteLine(ex.Message);
            }
        }

        private Task<bool> GetUserRole(string groupName)
        {
            ComplexProcedures c = new(_context);
            return c.IsRoleManager(groupName);
        }

        /// <summary>
        /// Remove a client from SignalR connection group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// SignalR on connection
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// SignalR on disconnected
        /// Clear connected users and managers
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // remove agents
            string removedItem;
            bool result = ConnectedUser.Ids.TryRemove(Context.ConnectionId, out removedItem);

            // remove managers
            result = ConnectedManagers.Ids.TryRemove(Context.ConnectionId, out removedItem);

            await base.OnDisconnectedAsync(ex);
        }
    }

    /// <summary>
    /// Connected clients collection
    /// </summary>
    public static class ConnectedUser
    {
        public static ConcurrentDictionary<string, string> Ids = new();
    }

    /// <summary>
    /// Connected manager collection
    /// </summary>
    public static class ConnectedManagers
    {
        public static ConcurrentDictionary<string, string> Ids = new();
    }

    /// <summary>
    /// Visitors waiting collection
    /// </summary>
    public static class WaitedVisitors
    {
        public static ConcurrentDictionary<int, VisitorWaitingTimer> visitorsWaiting = new();
    }
    // end namespace
}
