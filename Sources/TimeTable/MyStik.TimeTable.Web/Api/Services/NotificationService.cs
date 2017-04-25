using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class NotificationService
    {
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();
        private ApplicationDbContext _db = new ApplicationDbContext();

        // Persönliche Notifications abfragen
        public IEnumerable<NotificationContract> GetPersonalNotifications(string userId)
        {
            List<NotificationContract> contracts = new  List<NotificationContract>();
            
            List<ActivityDateChange> data = new List<ActivityDateChange>();

            data = Db.DateChanges.Where(a => a.NotificationStates.Any(b => b.UserId.Equals(userId))
                && DateTime.Compare(DateTime.Now, a.NewEnd) < 0)
                .OrderByDescending(a => a.NotificationStates.FirstOrDefault(n => n.UserId.Equals(userId)).IsNew).ToList();

            foreach (ActivityDateChange adc in data){
                contracts.Add(new NotificationContract(){

                    NotificationStateId = adc.NotificationStates.FirstOrDefault(x => x.UserId.Equals(userId)).Id.ToString(),
                    UserId = userId,
                    NotificationContent = adc.NotificationContent,
                    isNew = adc.NotificationStates.FirstOrDefault(x => x.UserId.Equals(userId)).IsNew,
                    ChangeId = adc.Id.ToString(),
                    Timestamp = adc.TimeStamp,
                });
            }

            return contracts.OrderBy(x => x.Timestamp);
        }

        // Persönlichen Token in der DB hinterlegen
        public TokenRegistryResponse SaveToken(string userId, string token, string deviceName)
        {
            // Überprüfen, ob der User in der DB hinterlegt ist
            var currentUser = _db.Users.SingleOrDefault(u => u.Id.Equals(userId));
            if (currentUser == null)
            {
                return new TokenRegistryResponse() { 
                tokenSaved = false
                };
            }

            // Überprüfen, ob der Token bereits in der DB hinterlegt ist            
            var existingUser = _db.Users.FirstOrDefault(x => x.Devices.Any(y => y.DeviceId.Equals(token)));

            if(existingUser != null){

                // Falls der Token schon hinterlegt ist, werden die UserIds verglichen
                // Wenn die UserId nicht mit der neuen übereinstimmt wird der token bei der neuen UserId hinterlegt und bei der alten gelöscht
                
                if(!existingUser.Id.Equals(currentUser.Id)) {

                    // das Device, dass ich entfernen will
                    var deviceToDelete = existingUser.Devices.FirstOrDefault(x => x.DeviceId.Equals(token));
                    // Token beim alten User entfernen
                    existingUser.Devices.Remove(deviceToDelete);

                    // Token beim neuen User hinzufügen
                    var device = new UserDevice(){
                        DeviceId = token,
                        DeviceName = deviceName,
                        Registered = DateTime.Now,
                    };
                    currentUser.Devices.Add(device);
                    _db.Devices.Add(device);
                }
            }
            else
            {
                // Token beim neuen User hinzufügen
                var device = new UserDevice()
                {
                    DeviceId = token,
                    DeviceName = deviceName,
                    Registered = DateTime.Now,
                };
                currentUser.Devices.Add(device);
                _db.Devices.Add(device);
            }
            _db.SaveChanges();
            return new TokenRegistryResponse()
            {
                tokenSaved = true
            };
        }
    }
}