using MeetingRoomBooking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.Managers {
    public class ChartDataManager {

        private readonly MeetingRoomBookingDbContext _context;

        public ChartDataManager(MeetingRoomBookingDbContext context) {
            _context = context;
        }

        public int[] GetBarChartData() {
            int year = DateTime.Now.Year;

            int[] returnData = new int[12];

            var data = _context.BookingInstance
                .Where(b => b.MeetingDate.Year == year)
                .ToList();

            for( int i = 0; i < 12; i++) {
                returnData[i] = data.Where(d => d.MeetingDate.Month == (1+i))
                                    .Count();
            }
            return returnData;
        }







    }
}
