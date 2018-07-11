using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CustomCalendarPoc.UserControls
{
    public interface IDayClickListener
    {
        void onDayClick(View view);
    }

    public class CustomCalendar : LinearLayout
    {
        private static string color_calendar_number = "#424A56";
        private static string color_pink = "#FF4081";
        private static string color_grey = "#a0a0a0";
        private static string color_green = "#388E3C";

        private static string[] ENG_MONTH_NAMES = {"January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"};

        private TextView currentDate;
        private TextView currentMonth;
        private Button selectedDayButton;
        private Button[] days;
        LinearLayout weekOneLayout;
        LinearLayout weekTwoLayout;
        LinearLayout weekThreeLayout;
        LinearLayout weekFourLayout;
        LinearLayout weekFiveLayout;
        LinearLayout weekSixLayout;
        private LinearLayout[] weeks;

        private int currentDateDay, chosenDateDay, currentDateMonth,
                chosenDateMonth, currentDateYear, chosenDateYear,
                pickedDateDay, pickedDateMonth, pickedDateYear;
        int userMonth, userYear;
        private IDayClickListener mListener;
        private Drawable userDrawable;

        private Calendar calendar;
        private DateTime dateTime;

        LayoutParams defaultButtonParams;
        private LayoutParams userButtonParams;

        public CustomCalendar(Context context) : base(context)
        {
            Initialize(context);
        }
        public CustomCalendar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context);
        }

        public CustomCalendar(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        // @RequiresApi(api = Build.VERSION_CODES.LOLLIPOP)
        //public CustomCalendar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
        //    : base(context, attrs, defStyleAttr, defStyleRes)
        //{
        //    Initialize(context);
        //}

        private void Initialize(Context context)
        {
            DisplayMetrics metrics = Resources.DisplayMetrics;

            var inflater = LayoutInflater.FromContext(context);
            View view = inflater.Inflate(Resource.Layout.CustomCalendar, this, true);

            calendar = Calendar.GetInstance(Java.Util.Locale.English);
            //dateTime = DateTime.Now;

            weekOneLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_1);
            weekTwoLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_2);
            weekThreeLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_3);
            weekFourLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_4);
            weekFiveLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_5);
            weekSixLayout = FindViewById<LinearLayout>(Resource.Id.calendar_week_6);
            currentDate = FindViewById<TextView>(Resource.Id.current_date);
            currentMonth = FindViewById<TextView>(Resource.Id.current_month);

            currentDateDay = chosenDateDay = calendar.Get(CalendarField.DayOfMonth);
            //currentDateDay = chosenDateDay = dateTime.Day;

            if (userMonth != 0 && userYear != 0)
            {
                currentDateMonth = chosenDateMonth = userMonth;
                currentDateYear = chosenDateYear = userYear;
            }
            else
            {
                currentDateMonth = chosenDateMonth = calendar.Get(CalendarField.Month);
                //currentDateMonth = chosenDateMonth = dateTime.Month;

                currentDateYear = chosenDateYear = calendar.Get(CalendarField.Year);
                //currentDateYear = chosenDateYear = dateTime.Year;
            }

            currentDate.Text = currentDateDay.ToString();
            currentMonth.Text = ENG_MONTH_NAMES[currentDateMonth];

            initializeDaysWeeks();
            if (userButtonParams != null)
            {
                defaultButtonParams = userButtonParams;
            }
            else
            {
                defaultButtonParams = getdaysLayoutParams();
            }
            addDaysinCalendar(defaultButtonParams, context, metrics);

            InitCalendarWithDate(chosenDateYear, chosenDateMonth, chosenDateDay);
        }


        private void initializeDaysWeeks()
        {
            weeks = new LinearLayout[6];
            days = new Button[6 * 7];

            weeks[0] = weekOneLayout;
            weeks[1] = weekTwoLayout;
            weeks[2] = weekThreeLayout;
            weeks[3] = weekFourLayout;
            weeks[4] = weekFiveLayout;
            weeks[5] = weekSixLayout;
        }


        private void InitCalendarWithDate(int year, int month, int day)
        {
            if (calendar == null)
                calendar = Calendar.GetInstance(Java.Util.Locale.English);

            calendar.Set(year, month, day);

            //  dateTime = new DateTime(year, month, day);



            int daysInCurrentMonth = calendar.GetActualMaximum(CalendarField.DayOfMonth);

            //int daysInCurrentMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            chosenDateYear = year;
            chosenDateMonth = month;
            chosenDateDay = day;

            calendar.Set(year, month, 1);
            int firstDayOfCurrentMonth = calendar.Get(CalendarField.DayOfWeek);

            calendar.Set(year, month, daysInCurrentMonth); // ?????

            int dayNumber = 1;
            int daysLeftInFirstWeek = 0;
            int indexOfDayAfterLastDayOfMonth = 0;

            if (firstDayOfCurrentMonth != 1)
            {
                daysLeftInFirstWeek = firstDayOfCurrentMonth;
                indexOfDayAfterLastDayOfMonth = daysLeftInFirstWeek + daysInCurrentMonth;
                for (int i = firstDayOfCurrentMonth; i < firstDayOfCurrentMonth + daysInCurrentMonth; ++i)
                {
                    if (currentDateMonth == chosenDateMonth
                            && currentDateYear == chosenDateYear
                            && dayNumber == currentDateDay)
                    {
                        //days[i].SetBackgroundColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.pink)));
                        days[i].SetBackgroundColor(Color.ParseColor(color_pink));
                        days[i].SetTextColor(Color.White);

                    }
                    else
                    {
                        days[i].SetTextColor(Color.Black);
                        days[i].SetBackgroundColor(Color.Transparent);
                    }

                    int[] dateArr = new int[3];
                    dateArr[0] = dayNumber;
                    dateArr[1] = chosenDateMonth;
                    dateArr[2] = chosenDateYear;
                    days[i].Tag = dateArr;
                    days[i].Text = dayNumber.ToString();

                    days[i].Click += (sender, e) =>
                    {
                        // Perform action on click
                        onDayClick(sender as View);
                    };

                    ++dayNumber;
                }
            }
            else
            {
                daysLeftInFirstWeek = 8;
                indexOfDayAfterLastDayOfMonth = daysLeftInFirstWeek + daysInCurrentMonth;
                for (int i = 8; i < 8 + daysInCurrentMonth; ++i)
                {
                    if (currentDateMonth == chosenDateMonth
                            && currentDateYear == chosenDateYear)
                    {
                        //days[i].SetBackgroundColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.pink)));
                        if (dayNumber < currentDateDay)
                        {
                            // days[i].SetBackgroundColor(Color.ParseColor(color_pink));
                            days[i].SetTextColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.dayGrey)));
                        }
                        else
                        {
                            days[i].SetTextColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.dayGreen)));

                            if (dayNumber == 15 || dayNumber == 16 || dayNumber == 22 || dayNumber == 23)
                            {
                                days[i].SetTextColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.dayRed)));
                                days[i].SetBackgroundResource(Resource.Drawable.unavailableStrikethrough);
                            }

                            if (dayNumber == 14 || dayNumber == 31 || dayNumber == 30)
                            {
                                days[i].SetTextColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.dayAmber)));
                                days[i].SetBackgroundResource(Resource.Drawable.standbyStrikethrough);
                            }  
                        }

                    }
                    //else
                    //{
                    //    days[i].SetTextColor(Color.Black);
                    //    days[i].SetBackgroundColor(Color.Transparent);
                    //}



                    int[] dateArr = new int[3];
                    dateArr[0] = dayNumber;
                    dateArr[1] = chosenDateMonth;
                    dateArr[2] = chosenDateYear;
                    days[i].Tag = dateArr;
                    days[i].Text = dayNumber.ToString();
                    days[i].Click += (sender, e) =>
                    {
                        // Perform action on click
                        onDayClick(sender as View);
                    };

                    ++dayNumber;
                }
            }

            if (month > 0)
                calendar.Set(year, month - 1, 1);
            else
                calendar.Set(year - 1, 11, 1);

            #region previousMonthDays
            //int daysInPreviousMonth = calendar.GetActualMaximum(CalendarField.DayOfMonth);

            //for (int i = daysLeftInFirstWeek - 1; i >= 0; --i)
            //{
            //    int[] dateArr = new int[3];

            //    if (chosenDateMonth > 0)
            //    {
            //        if (currentDateMonth == chosenDateMonth - 1
            //                && currentDateYear == chosenDateYear
            //                && daysInPreviousMonth == currentDateDay)
            //        {
            //        }
            //        else
            //        {
            //            days[i].SetBackgroundColor(Color.Transparent);
            //        }

            //        dateArr[0] = daysInPreviousMonth;
            //        dateArr[1] = chosenDateMonth - 1;
            //        dateArr[2] = chosenDateYear;
            //    }
            //    else
            //    {
            //        if (currentDateMonth == 11
            //                && currentDateYear == chosenDateYear - 1
            //                && daysInPreviousMonth == currentDateDay)
            //        {
            //        }
            //        else
            //        {
            //            days[i].SetBackgroundColor(Color.Transparent);
            //        }

            //        dateArr[0] = daysInPreviousMonth;
            //        dateArr[1] = 11;
            //        dateArr[2] = chosenDateYear - 1;
            //    }

            //    days[i].Tag = dateArr;
            //    days[i].Text = (daysInPreviousMonth--).ToString();
            //    days[i].Click += (sender, e) =>
            //    {
            //        // Perform action on click
            //        onDayClick(sender as View);
            //    };             
            //}
            #endregion

            #region nextMonthDays
            //int nextMonthDaysCounter = 1;
            //for (int i = indexOfDayAfterLastDayOfMonth; i < days.Length; ++i)
            //{
            //    int[] dateArr = new int[3];

            //    if (chosenDateMonth < 11)
            //    {
            //        if (currentDateMonth == chosenDateMonth + 1
            //                && currentDateYear == chosenDateYear
            //                && nextMonthDaysCounter == currentDateDay)
            //        {
            //            days[i].SetBackgroundColor(Color.ParseColor(color_pink));
            //        }
            //        else
            //        {
            //            days[i].SetBackgroundColor(Color.Transparent);
            //        }

            //        dateArr[0] = nextMonthDaysCounter;
            //        dateArr[1] = chosenDateMonth + 1;
            //        dateArr[2] = chosenDateYear;
            //    }
            //    else
            //    {
            //        if (currentDateMonth == 0
            //                && currentDateYear == chosenDateYear + 1
            //                && nextMonthDaysCounter == currentDateDay)
            //        {
            //            // days[i].SetBackgroundColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.pink)));
            //            days[i].SetBackgroundColor(Color.ParseColor(color_pink));
            //        }
            //        else
            //        {
            //            days[i].SetBackgroundColor(Color.Transparent);
            //        }

            //        dateArr[0] = nextMonthDaysCounter;
            //        dateArr[1] = 0;
            //        dateArr[2] = chosenDateYear + 1;
            //    }

            //    days[i].Tag =dateArr;
            //    days[i].SetTextColor(Color.ParseColor(color_grey));
            //    days[i].Text = (nextMonthDaysCounter++).ToString();
            //    days[i].Click += (sender, e) =>
            //    {
            //        // Perform action on click
            //        onDayClick(sender as View);
            //    };               
            //}

            #endregion

            calendar.Set(chosenDateYear, chosenDateMonth, chosenDateDay);
        }

        public void onDayClick(View view)
        {
            // mListener.onDayClick(view);

            if (selectedDayButton != null)
            {
                if (chosenDateYear == currentDateYear
                        && chosenDateMonth == currentDateMonth
                        && pickedDateDay == currentDateDay)
                {
                    // selectedDayButton.SetBackgroundColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.pink)));
                    selectedDayButton.SetBackgroundColor(Color.ParseColor(color_pink));
                    selectedDayButton.SetTextColor(Color.White);
                }
                else
                {
                    selectedDayButton.SetBackgroundColor(Color.Transparent);
                    if (selectedDayButton.CurrentTextColor != Color.Red)
                    {
                        //selectedDayButton.SetTextColor(new Color(ContextCompat.GetColor(this.Context, Resource.Color.calendar_number)));
                        selectedDayButton.SetTextColor(Color.ParseColor(color_calendar_number));
                    }
                }
            }

            selectedDayButton = (Button)view;
            if (selectedDayButton.Tag != null)
            {
                int[] dateArray = (int[])selectedDayButton.Tag;
                pickedDateDay = dateArray[0];
                pickedDateMonth = dateArray[1];
                pickedDateYear = dateArray[2];
            }

            if (pickedDateYear == currentDateYear
                    && pickedDateMonth == currentDateMonth
                    && pickedDateDay == currentDateDay)
            {

                selectedDayButton.SetBackgroundColor(Color.ParseColor(color_pink));
                selectedDayButton.SetTextColor(Color.White);
            }
            else
            {
                //selectedDayButton.SetBackgroundColor(Color.ParseColor(color_grey));
                //if (selectedDayButton.CurrentTextColor != Color.Red)
                //{
                //    selectedDayButton.SetTextColor(Color.White);
                //}
                selectedDayButton.SetBackgroundResource(Resource.Drawable.daySelector);
            }
        }


        private void addDaysinCalendar(LayoutParams buttonParams, Context context,
                                       DisplayMetrics metrics)
        {
            int engDaysArrayCounter = 0;

            for (int weekNumber = 0; weekNumber < 6; ++weekNumber)
            {
                for (int dayInWeek = 0; dayInWeek < 7; ++dayInWeek)
                {
                    Button day = new Button(context);
                    day.SetTextColor(Color.ParseColor(color_grey));
                    day.SetBackgroundColor(Color.Transparent);
                    day.LayoutParameters = buttonParams;
                    day.SetTextSize(ComplexUnitType.Dip, (int)metrics.Density * 8);
                    day.SetSingleLine();

                    days[engDaysArrayCounter] = day;
                    weeks[weekNumber].AddView(day);

                    ++engDaysArrayCounter;
                }
            }
        }


        private LayoutParams getdaysLayoutParams()
        {
            LinearLayout.LayoutParams buttonParams = new LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent);
            buttonParams.Weight = 1;
            return buttonParams;
        }

        public void setUserDaysLayoutParams(LinearLayout.LayoutParams userButtonParams)
        {
            this.userButtonParams = userButtonParams;
        }
        public void setUserCurrentMonthYear(int userMonth, int userYear)
        {
            this.userMonth = userMonth;
            this.userYear = userYear;
        }
        public void setDayBackground(Drawable userDrawable)
        {
            this.userDrawable = userDrawable;
        }

        public void setCallBack(IDayClickListener mListener)
        {
            this.mListener = mListener;
        }
    }
}