# Meeting Room Booking Web App

Welcome to the **Meeting Room Booking Web App**, a user-friendly platform for scheduling and managing meeting room reservations.

## Features

- **User Authentication**: Login system for users and admins.
- **User Management**: Add, edit, or delete meeting users.
- **Room Management**: Add, edit, or delete meeting rooms and amenities (e.g., projectors, whiteboards).
- **Booking System**: Reserve rooms with real-time availability checks.
- **Recurring Bookings**: Create recurring reservations (daily, weekly, or monthly).
- **Search and Filter**: Filter meeting rooms based on facilities.

## Technologies Used

- **Framework**: ASP.NET MVC (Model-View-Controller)
- **Language**: C#
- **Frontend**: Razor Views, Bootstrap, jQuery, FullCalendar, ChartJs
- **Database**: Microsoft SQL
- **ORM**: Entity Framework Core
- **Tools**: Visual Studio 2022, Git

## Getting Started

### Prerequisites

Ensure you have the following installed:

- Visual Studio 2022 (or later)
- .NET SDK 6.0 (or later)
- Microsoft SQL
- Git

### Setup

1. Clone the repository:
   https://github.com/Alonekyot/ASIJumpstart2024-Group2.git
   Use the master branch
2. Restore Database BackUp  
   MeetingRoomBookingDb.bak

   Follow these steps to restore the database backup using SQL Server Management Studio (SSMS):

   1. **Open SQL Server Management Studio (SSMS):**

      - Launch SSMS and connect to your SQL Server instance.

   2. **Open the Restore Database Wizard:**

      - In the Object Explorer, right-click on the **Databases** node.
      - Select **Restore Database...** from the context menu.

   3. **Choose the Source:**

      - In the **Restore Database** window:
        - Under the **Source** section, select **Device**.
        - Click the **Browse** (`...`) button to locate your backup file.

   4. **Select the Backup File:**

      - In the **Select backup devices** dialog:
        - Click **Add...** and navigate to the folder containing your `MeetingRoomBookingDb.bak` file.
        - Select the backup file and click **OK**.

   5. **Configure Restore Options:**

      - In the **Destination** section:
        - Ensure the **Database** name matches the application's configuration (e.g., `MeetingRoomBookingDb`).
      - Under the **Select the backup sets to restore** section, ensure the checkbox for your backup set is selected.

   6. **Check File Paths (Optional):**

      - Go to the **Files** page in the left-hand menu to verify or change the file locations for the database and log files.

   7. **Perform the Restore:**

      - Click **OK** to start the restore process.
      - Once completed, a confirmation message will appear.

   8. **Update Connection String (if needed):**
      - Ensure your application’s connection string in the `appsettings.json` file points to the restored database. Example:
        ```json
        "ConnectionStrings": {
            "DefaultConnection": "Server=YOUR_SERVER;Database=MeetingRoomBookingDb;Trusted_Connection=True;"
        }
        ```

3. Set MeetingRoomBooking.WebApp as the Startup Project
