﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportClub.BLL.DTO;
using SportClub.BLL.Interfaces;
using SportClub.BLL.Services;
using SportClub.Models;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace SportClub.Controllers
{
    public class TimeController : Controller
    {
        private readonly IAdmin adminService;
        private readonly IRoom roomService;
        private readonly IUser userService;
        private readonly ICoach coachService;
        private readonly ITime timeService;
        private readonly ITimetable timetableService;
        private readonly IShedule sheduleService;
        private readonly ISpeciality specialityService;
        private readonly ITrainingInd trainingIndService;
        private static List<TimeTDTO> timesT=new();
        private static List<TimetableDTO> timetables = new();
      //  private static ITrainingInd trainingIndService ;

        public TimeController(ITrainingInd ti,IShedule sh,IRoom room,IAdmin adm, IUser us, ICoach c, ISpeciality sp, ITime t, ITimetable timetableService)
        {
            adminService = adm;
            userService = us;
            coachService = c;
            timeService = t;
            specialityService = sp;
            roomService=room;
            this.timetableService = timetableService;
            sheduleService = sh;
            trainingIndService = ti;
        }
        [HttpGet]
        public async Task<IActionResult> AddTimeT()
        {
            await PutTimes();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTimeT(string Start,string End)
        {
            try
            {
                await timeService.AddTimeT(Start, End);
            }
            catch { }
            await PutTimes();
            return View();
        }
        public async Task PutTimes()
        {
            IEnumerable<TimeTDTO> p = await timeService.GetAllTimeTs();
            
            IEnumerable<TimeTDTO> p2 = p.OrderBy(x => double.Parse(x.StartTime.Split(':')[0]+","+ x.StartTime.Split(':')[1]));
            List<TimeShow> p1 = new();
            foreach (var t in p2)
            {
                TimeShow ts = new()
                {
                    Id = t.Id,
                    Time = t.StartTime + "/" + t.EndTime
                };
                p1.Add(ts);
            }
            p1.OrderBy(x => x.Time).ToList();
            ViewData["TimeId"] = new SelectList(p1, "Id", "Time");
        }
        public async Task<IActionResult> EditTime(int Id)
        {
           // HttpContext.Session.SetString("path", Request.Path);
            TimeTDTO p = await timeService.GetTimeT(Id);
            if (p != null)
            {
               
                return View("EditTimeT",p);
            }
            await PutTimes();
            return View("AddTimeT");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTime( TimeTDTO t, string Start, string End)
        {
          //  HttpContext.Session.SetString("path", Request.Path);
            try
            {
                TimeTDTO p = await timeService.GetTimeT(t.Id);
                if (p == null)
                {
                    await PutTimes();
                    return View("AddTimeT");
                }
                p.StartTime = Start;
                p.EndTime = End;
                await timeService.UpdateTimeT(p);
                await PutTimes();
                return View("AddTimeT");
            }
            catch
            {
                await PutTimes();
                return View("AddTimeT");
            }
        }
        public async Task<IActionResult> DeleteTime(int Id)
        {
            // HttpContext.Session.SetString("path", Request.Path);
            TimeTDTO p = await timeService.GetTimeT(Id);
            if (p != null)
            {
               
                return View("DeleteTimeT", p);
            }
            await PutTimes();
            return View("AddTimeT");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteTime(TimeTDTO t)
        {
            //  HttpContext.Session.SetString("path", Request.Path);
            try
            {
                TimeTDTO p = await timeService.GetTimeT(t.Id);
                if (p == null)
                {
                    await PutTimes();
                    return View("AddTimeT");
                }               
                await timeService.DeleteTimeT(p.Id);
                await PutTimes();
                return View("AddTimeT");
            }
            catch
            {
                await PutTimes();
                return View("AddTimeT");
            }
        }
        [HttpGet]
      
         public async Task<IActionResult> AddTimetable()
         {           
             await PutTimes();
             return View();
         }
         public async Task<IActionResult> AddTimesToTable(int id)
         {
             TimeTDTO p = await timeService.GetTimeT(id);
             timesT.Add(p);
             await PutTimes();
              PutTimesToTable();
             return View("AddTimetable");
         }
         [HttpPost]
         public async Task<IActionResult> AddTimeTable()
         {
             if (timesT.Count > 0)
             {
                 TimetableDTO t = new();
                 foreach (var time in timesT)
                     t.TimesId.Add(time.Id);
                 await timetableService.AddTimetable(t);
                 timesT.Clear();
                 return Redirect("/Home/Index"); 
             }
             else
             {
                await PutTimes();
                PutTimesToTable();
                 return View();
             }
         }
        [HttpPost]
        public async Task<IActionResult> Cancel()
        {
            if (timesT.Count > 0)
            {              
                timesT.RemoveAt(timesT.Count-1);
                await PutTimes();
                PutTimesToTable();
                return View("AddTimetable");              
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }
        [HttpPost]
        public  IActionResult Exit()
        {
            timesT.Clear();
            return Redirect("/Home/Index");            
        }
        public void PutTimesToTable()
         {

             List<TimeShow> p1 = new();
             IEnumerable<TimeTDTO> p2 = timesT.OrderBy(x => int.Parse(x.StartTime.Split(':')[0]));
             foreach (var t in p2)
             {
                 TimeShow ts = new()
                 {
                     Id = t.Id,
                     Time = t.StartTime + "/" + t.EndTime
                 };
                 p1.Add(ts);
             }
             ViewData["TimetableId"] = new SelectList(p1, "Id", "Time");
         }
        [HttpGet]
        public async Task <IActionResult> ChoseRomm()
        {
            IEnumerable<RoomDTO> r = await roomService.GetAllRooms();
            ViewData["RoomsId"] = new SelectList(r, "Id", "Name");
            return View("ChoseRomm");
        }
        [HttpPost]
        public async Task<IActionResult> GetAllTimetable(int Id)
        {
            List<TimetableShow> ts = new();
            IEnumerable<TimetableDTO> p = await timetableService.GetAllTimetables();
            foreach(var t in p)
            {
                TimetableShow t1 = new();
                t1.Id = t.Id;
               
                foreach (int i in t.TimesId)
                {
                    TimeTDTO td = await timeService.GetTimeT(i);
                    string st = td.StartTime + "/" + td.EndTime;
                    t1.Times.Add(st);
                }
                ts.Add(t1);
            }
           // IEnumerable<RoomDTO> r= await roomService.GetAllRooms();
           RoomDTO r=await roomService.GetRoom(Id);
            MakeSheduleView m = new();
            m.times = ts;
            m.room = r;
           // ViewData["RoomsId"] = new SelectList(r, "Id", "Name");
            // return View("GetTimetables",ts);
            return View("GetTimetables", m);
        }
        [HttpPost]
        public async Task<IActionResult> AddTimetableToShedule(int id, int roomId)
        {
            if (id != 0)
            {
                TimetableDTO tt = await timetableService.GetTimetable(id);
                if (timetables.Count < 7)
                    timetables.Add(tt);
            }
            else
            {
                TimetableDTO tt1=new();
                tt1.Id= id;
                timetables.Add(tt1);
            }
            RoomDTO r = await roomService.GetRoom(roomId);
            List<TimetableShow> ts = new();
            IEnumerable<TimetableDTO> p = await timetableService.GetAllTimetables();
            foreach (var t in p)
            {
                TimetableShow t1 = new();
                t1.Id = t.Id;

                foreach (int i in t.TimesId)
                {
                    TimeTDTO td = await timeService.GetTimeT(i);
                    string st = td.StartTime + "/" + td.EndTime;
                    t1.Times.Add(st);
                }
                ts.Add(t1);
            }
            List<TimetableShow> ts2 = new();        
            foreach (var t in timetables)
            {
                TimetableShow t1 = new();
                t1.Id = t.Id;
                if (t.TimesId.Count == 0)
                {
                    string s = "Выходной";
                    t1.Times.Add(s);
                }
                else
                {
                    foreach (int i in t.TimesId)
                    {
                        TimeTDTO td = await timeService.GetTimeT(i);
                        string st = td.StartTime + "/" + td.EndTime;
                        t1.Times.Add(st);
                    }
                }
                ts2.Add(t1);
            }

            MakeSheduleView m = new();
            m.times = ts;
            m.room = r;
            m.timesAdded = ts2;
            return View("GetTimetables", m);
        }
        [HttpPost]
        public async Task<IActionResult> SaveShedule( int rId)
        {
            SheduleDTO shedule = new SheduleDTO();
            RoomDTO r = await roomService.GetRoom(rId);
            foreach (var t in timetables)
            {
                
                shedule.timetables.Add(t);
            }
           await sheduleService.AddShedule(shedule,r);
            timetables.Clear();
            return RedirectToAction("Room_Shedule");
        }
        [HttpGet]
        public async Task<IActionResult> Room_Shedule()
        {
            IEnumerable<RoomDTO> r = await roomService.GetAllRooms();
            ViewData["RoomsId"] = new SelectList(r, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoomWithShedule(int Id)
        {
            RoomDTO room =await roomService.GetRoom(Id);
            SheduleDTO shDto = null;
            try
            {
                shDto = await sheduleService.GetShedule(room.sheduleId.Value);
            }
            catch { }
            MakeSheduleView m = new();
            m.room = room;
           // m.shedule = room.sheduleId;
            if (shDto != null)
            {
                m.times = new();
                foreach (var t in shDto.timetables)
                {
                    TimetableShow t1 = new();
                    t1.Id = t.Id;

                    if (t.TimesId.Count == 0)
                    {
                        string s = "Выходной";
                        t1.Times.Add(s);
                    }
                    else
                    {
                        foreach (int i in t.TimesId)
                        {
                            TimeTDTO td = await timeService.GetTimeT(i);
                            string st = td.StartTime + "/" + td.EndTime;
                            t1.Times.Add(st);
                        }
                    }
                    m.times.Add(t1);
                }
            }
            else
            {
                m.message = "для зала не составлен график";
            }
            return View(m);
        }
        [HttpGet]
        public async Task<IActionResult> AddIndTraining(int dayS, int roomId, string timeS, string roomName)
        {
            TrainingIndDTO training = new();
            training.RoomId = roomId;
            training.Time = timeS;
            training.Day = dayS;
            training.RoomName=roomName;
            if (dayS == 0) training.DayName = "Понедельник";
            else if (dayS == 1) training.DayName = "Вторник";
            else if (dayS == 2) training.DayName = "Среда";
            else if (dayS == 3) training.DayName = "Четверг";
            else if (dayS == 4) training.DayName = "Пятница";
            else if (dayS == 5) training.DayName = "Суббота";
            else if (dayS == 6) training.DayName = "Воскресенье";
            IEnumerable<CoachDTO> p = await coachService.GetAllCoaches();
            ViewData["CoachId"] = new SelectList(p, "Id", "Name");
           // await trainingIndService.AddTrainingInd(training);
            return View(training);
        }
       /* [HttpPost]
        public async Task<IActionResult> AddingToTrainingInd(int day, int roomId, string time, string roomName, int coachId,int userId)
        {
            TrainingIndDTO tr = new();
            tr.CoachId = coachId;
            tr.RoomId = roomId;
            tr.RoomName = roomName;
            tr.Day = day;
            tr.Time = time;
            if (userId != 0)
            {
                UserDTO user = await userService.GetUser(userId);
                tr.UserId = user.Id;
                tr.UserName = user.Name;
            }
           
            await trainingIndService.AddTrainingInd(tr);
            return View("AddIndTraining",tr);
        }*/
        [HttpGet]
        public async Task<IActionResult> AddTrainingI(int Id,int day, int roomId, string time, string roomName)
        {
            TrainingIndDTO training = new TrainingIndDTO();
            training.Id = Id;
            training.RoomId = roomId;
            training.CoachId = Id;
            training.Time=time;
            training.Day = day;
            training.RoomName = roomName;
            await trainingIndService.AddTrainingInd(training);
            return View();
        }

    }
}
