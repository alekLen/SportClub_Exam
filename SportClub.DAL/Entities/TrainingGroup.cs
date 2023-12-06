﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportClub.DAL.Entities
{
    public class TrainingGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeT Time { get; set; }
        public int Day { get; set; }
        public Room Room { get; set; }
        public Coach Coach { get; set; }
        public Group Group { get; set; }
        public int Number { get; set; }
        public Speciality Speciality { get; set; }
        public bool available { get; set; } = true;
    }
}
