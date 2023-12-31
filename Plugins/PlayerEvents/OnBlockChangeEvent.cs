/*
	Copyright � 2011-2014 MCForge_Redux-Redux
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;

namespace MCForge_Redux
{
    /// <summary>
    /// This event is called whenever a player places or deletes a block
    /// </summary>
    public class OnBlockChangeEvent
    {
        internal static List<OnBlockChangeEvent> events = new List<OnBlockChangeEvent>();
        Plugin plugin;
        Player.BlockchangeEventHandler method;
        Priority priority;
        internal OnBlockChangeEvent(Player.BlockchangeEventHandler method, Priority priority, Plugin plugin) { this.plugin = plugin; this.priority = priority; this.method = method; }
        public static void Call(Player p, ushort x, ushort y, ushort z, ushort type)
        {
            events.ForEach(delegate(OnBlockChangeEvent p1)
            {
                try
                {
                    p1.method(p, x, y, z, type);
                }
                catch (Exception e) { Server.s.Log("The plugin " + p1.plugin.name + " errored when calling the PlayerBlockChange Event!"); Server.ErrorLog(e); }
            });
        }
        static void Organize()
        {
            List<OnBlockChangeEvent> temp = new List<OnBlockChangeEvent>();
            List<OnBlockChangeEvent> temp2 = events;
            OnBlockChangeEvent temp3 = null;
            int i = 0;
            int ii = temp2.Count;
            while (i < ii)
            {
                foreach (OnBlockChangeEvent p in temp2)
                {
                    if (temp3 == null)
                        temp3 = p;
                    else if (temp3.priority < p.priority)
                        temp3 = p;
                }
                temp.Add(temp3);
                temp2.Remove(temp3);
                temp3 = null;
                i++;
            }
            events = temp;
        }
        public static OnBlockChangeEvent Find(Plugin plugin)
        {
            foreach (OnBlockChangeEvent p in events.ToArray())
            {
                if (p.plugin == plugin)
                    return p;
            }
            return null;
        }
        /// <summary>
        /// Register this event
        /// </summary>
        /// <param name="method">This is the delegate that will get called when this event occurs</param>
        /// <param name="priority">The priority (imporantce) of this call</param>
        /// <param name="plugin">The plugin object that is registering the event</param>
        /// <param name="bypass">Register more than one of the same event</param>
        public static void Register(Player.BlockchangeEventHandler method, Priority priority, Plugin plugin, bool bypass = false)
        {
            if (Find(plugin) != null)
                if (!bypass)
                throw new Exception("The user tried to register 2 of the same event!");
            events.Add(new OnBlockChangeEvent(method, priority, plugin));
            Organize();
        }
        /// <summary>
        /// UnRegister this event
        /// </summary>
        /// <param name="plugin">The plugin object that has this event registered</param>
        public static void UnRegister(Plugin plugin)
        {
            if (Find(plugin) == null)
                throw new Exception("This plugin doesnt have this event registered!");
            else
                events.Remove(Find(plugin));
        }
    }
}
