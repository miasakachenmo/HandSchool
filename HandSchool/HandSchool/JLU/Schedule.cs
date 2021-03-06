﻿using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class Schedule : IScheduleEntrance
    {
        public string Name => "课程表";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string LastReport { get; private set; }
        public List<CurriculumItem> Items { get; }
        public string StorageFile => "jlu.kcb.json";
        public string[] ClassBetween = { "8:00", "8:55", "10:00", "10:55", "13:30", "14:25", "15:30", "16:25", "18:30", "19:25", "20:20" };
        public string PostValue => "{\"tag\":\"teachClassStud@schedule\",\"branch\":\"default\",\"params\":{\"termId\":`term`,\"studId\":`studId`}}";

        public void RenderWeek(int week, out List<CurriculumItem> list, bool showAll = false)
        {
            if (showAll)
                throw new NotImplementedException();

            list = Items.FindAll((item) => showAll || item.IfShow(week));
        }

        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post(ScriptFileUri, PostValue);
            WriteConfFile(StorageFile, LastReport);
            Parse();
            Save();
        }
        
        public void Parse()
        {
            var table = JSON<RootObject<ScheduleValue>>(LastReport);
            Items.RemoveAll(obj => !obj.IsCustom);
            foreach (var obj in table.value)
            {
                foreach (var time in obj.teachClassMaster.lessonSchedules)
                {
                    var item = new CurriculumItem
                    {
                        WeekBegin = int.Parse(time.timeBlock.beginWeek),
                        WeekEnd = int.Parse(time.timeBlock.endWeek),
                        WeekOen = (WeekOddEvenNone)(time.timeBlock.weekOddEven == null ? 2 : (time.timeBlock.weekOddEven == "E" ? 1 : 0)),
                        WeekDay = int.Parse(time.timeBlock.dayOfWeek),
                        Classroom = time.classroom.fullName,
                        CourseID = obj.teachClassMaster.name,
                        SelectDate = obj.dateAccept,
                        Name = obj.teachClassMaster.lessonSegment.fullName,
                    };

                    foreach (var t in obj.teachClassMaster.lessonTeachers)
                        item.Teacher += t.teacher.name + " ";
                    item.Teacher = item.Teacher.Trim();

                    int tmp = int.Parse(time.timeBlock.classSet);
                    int tmp2 = tmp & (-tmp);
                    while (tmp != 0)
                    {
                        tmp >>= 1;
                        tmp2 >>= 1;
                        if (tmp2 > 1)
                            item.DayBegin++;
                        else if (tmp2 == 1)
                            item.DayEnd = ++item.DayBegin;
                        else if (tmp >= 1)
                            item.DayEnd++;
                    }

                    Items.Add(item);
                }
            }
        }

        /*
         * Experimental
        private string ConvertClassroom(string fullName)
        {
            string[] by = fullName.Split('-', 2)[1].Split('#');
            var buildingName = building[by[0]];
            var roomName = by[1];
            foreach (var keys in theater.AllKeys)
            {
                roomName = roomName.Replace(keys, theater[keys]);
            }
            return buildingName + " " + roomName;
             * Experimental
            building.Add("经信教学楼", "经信");
            building.Add("逸夫楼", "逸夫");
            building.Add("第三教学楼", "三教");
            building.Add("计算机新楼", "计算机楼");
            building.Add("体育场", "体育场");
            theater.Add("第一阶梯", "1阶");
            theater.Add("第二阶梯", "2阶");
            theater.Add("第三阶梯", "3阶");
            theater.Add("第四阶梯", "4阶");
            theater.Add("第五阶梯", "5阶");
            theater.Add("第六阶梯", "6阶");
            theater.Add("第七阶梯", "7阶");
            theater.Add("第八阶梯", "8阶");
            theater.Add("第九阶梯", "9阶");
            theater.Add("第十阶梯", "10阶");
            theater.Add("第十一阶梯", "11阶");
            theater.Add("第十二阶梯", "12阶");
            theater.Add("第十三阶梯", "13阶");
            theater.Add("第十四阶梯", "14阶");
            theater.Add("第十五阶梯", "15阶");
            theater.Add("A区", "A");
            theater.Add("B区", "B");
            theater.Add("C区", "C");
            theater.Add("D区", "D");
            theater.Add("E区", "E");
            theater.Add("F区", "F");
            theater.Add("课堂", "");
        }
        
        private NameValueCollection building = new NameValueCollection();
        private NameValueCollection theater = new NameValueCollection();
        */
        
        public void Save()
        {
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
            WriteConfFile("jlu.kcb2.json", Serialize(Items));
        }

        public int ClassNext
        {
            get
            {
                var ret = 0;

                for (int i = 0; i < 11; i++)
                {
                    if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(ClassBetween[i])) > 0)
                    {
                        ret += 1;
                    }
                }

                return ret;
            }
        }

        public Schedule()
        {
            LastReport = ReadConfFile("jlu.kcb2.json");
            if (LastReport != "")
                Items = JSON<List<CurriculumItem>>(LastReport);
            else
                Items = new List<CurriculumItem>();
            Items.Sort((x, y) => (x.WeekDay * 100 + x.DayBegin).CompareTo(y.WeekDay * 100 + y.DayBegin));
        }
    }
}
