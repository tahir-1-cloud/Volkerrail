var objCommon = new Common();
$(document).ready(function () {


    var app = new Vue({
        el: '#apporg',
        data: {
            DataList: [],
            HeaderCol: [],
            ShiftData: [],
            SelectedShiftData: [],
            d: {},
            department: '',
            LookupList: [],
            ShiftId: 0,
            Roster: {
                EmaployeeData: [],
                DepartmentData: [],
                RosterData: [],
                department: ''
            },
            Absance: {
                EmployeeId: 0,
                AbsId: 0,
                FromDate: "",
                Todate: "",
                LeaveType: "",
                Reason: ""
            },
            RosterShift: {
                StartDateTime: "",
                FinishDateTime: "",
                DriverName: "",
                ShiftLocation: "",
                Description: "",
                ShiftId: 0
            },
            comment: {
                ListArrangements: [],
                EngineeringSupport: "",
                CoursesAndOthers: ""
            },
            item: {

            },
        },
        methods: {
            Delete(id) {

            },
            Edit(data) {

            }
            ,
            ShowLookup($event) {

            },
            Insert($event) {
               
            },
            check($event) {
                console.log($event);
            }
            ,
            GetRosterData(type) {
                this.GetData(null);
            },
            GetData($event) {
                var dthis = this;
                var dxthis = this.Roster;
                $("#spanFromWeek").html($("#ddlFromWeek option:selected").text());
                $("#spanToWeek").html($("#ddlToWeek option:selected").text() != "" ? "-" + $("#ddlToWeek option:selected").text() : "");

                var roasterdata = {
                    department: $("#ddlMachineDepartment").val(),
                    FromWeek: Number($("#ddlFromWeek").val()),
                    ToWeek: Number($("#ddlToWeek").val())
                }
                objCommon.AjaxCall("/Roster/GetData", $.param(roasterdata), "GET", true, function (response) {
                   
               
                    dxthis.EmaployeeData = response.EmployeeData;
                    console.log("emp",response.EmployeeData);
                    dxthis.DepartmentData = response.DepartmentData;
                    dxthis.RosterData = response.RosterData;

                    dthis.comment = response.WeeklyComments;
                    if (response.WeeklyComments.ListArrangements == null) {
                        dthis.comment.ListArrangements = [];
                    }

                    var resource = [];
                    var evetns = [];

                 

                    dxthis.EmaployeeData.forEach(function (v) {
                        v.FullName = (v.FirstName == null ? "" : v.FirstName) + ' ' + (v.LastName == null ? "" : v.LastName);

                        //resource.push({ id: v.Id, employee: v.FullName, select: false, leader: false, fitter: false })
                        resource.push({ id: v.Id, employeeLastName: v.LastName, employee: v.FullName, select: false, leader: false, fitter: false })
                        
                    })
                  
                   
                    console.log(dxthis.RosterData);
                    dxthis.ShiftData = response.ShiftData;
                    var currentWeek = WeekList.filter(x => x.Id == Number($("#ddlFromWeek").val()))[0];
                    var defaultDatetime = "";
                    if (currentWeek != undefined)
                        defaultDatetime = currentWeek.StartDate;

                    dxthis.RosterData.forEach(function (v) {
                        // if (v.EventType == "Leave") {

                        evetns.push({
                            id: v.Id,
                            resourceId: v.EmployeeId,
                            title: v.EventType == 'Leave' ? v.Reason : v.Title,
                            start: v.StartDate,
                            end: v.EndDate,
                            shiftId: v.ShiftId,
                            eventType: v.EventType,
                            //  allDay: true,
                            color: v.Color,     // an option!
                            textColor: 'yellow', // an option!,
                            employeeName: v.EmployeeName,
                            reason: v.Title
                          
                        })
                        //}
                       
                    })
                    console.log("employeeName", evetns)

                    //var dateOnly = new Date();
                    //if (currentWeek != undefined) {
                    //    var dateObject = new Date(defaultDatetime);

                    //    // Extract the date only
                    //    dateOnly = dateObject.toISOString().split('T')[0];
                    //}
                    var dateOnly = new Date();
                    if (currentWeek != undefined) {

                        // var dateObject = new Date(defaultDatetime);
                        var parts = defaultDatetime.split('/');
                        var day = parseInt(parts[0], 10);
                        var month = parseInt(parts[1], 10) - 1;
                        var year = parseInt(parts[2], 10);

                        var dateObject = new Date(Date.UTC(year, month, day));

                        if (!isNaN(dateObject.getTime())) {
                            //var dateObject = new Date(defaultDatetime + " UTC");

                            //var dateObject = new Date(defaultDatetime);
                            //dateOnly = dateObject.toLocaleDateString("en-GB", { timeZone: "Europe/London" })
                            //var parsedDateObject = new Date(dateOnly.split('/').reverse().join('-'));
                            console.log("defaultdate", defaultDatetime);
                            dateOnly = dateObject.toLocaleDateString("en-GB", { timeZone: "Europe/London" });
                            console.log("Date only", dateOnly);
                            var parsedDateObject = new Date(dateObject);
                            console.log("parsedDate", parsedDateObject);
                            //console.log(dateOnly);
                        } else {

                            console.error('Invalid date:', defaultDatetime);
                        }
                    }
                    var calendarEl = document.getElementById('rosterGrid');
                    calendarEl.innerHTML = "";

                    var calendar = new FullCalendar.Calendar(calendarEl, {
                        schedulerLicenseKey: 'GPL-My-Project-Is-Open-Source',
                        plugins: ['resourceTimeline', 'interaction'],
                        stickyHeaderDates: true,
                        defaultDate: parsedDateObject,
                        header: {
                            left: 'today prev,next',
                            center: 'title',
                            right: 'resourceTimelineDay,resourceTimelineWeek,resourceTimelineMonth'
                        },
                        height: 600,
                        aspectRatio: 1.5,
                        defaultView: 'resourceTimelineMonth',
                        resourceAreaWidth: '20%',
                        resourceColumns: [
                            {
                                labelText: 'Employees',
                                field: 'employee'
                            }
                        ],
                        firstDay: 6,
                        views: {
                            resourceTimelineWeek: {
                                type: 'resourceTimeline',
                                duration: { weeks: 1 },
                                slotDuration: { days: 1 },

                                weekNumberCalculation: 'local',
                                weekNumberFormat: { week: 'short' },

                            }
                        },
                        resourceRender: function (info) {
                            //  console.log(info.resource.extendedProps);
                            // console.log(info);
                            // console.log(info.el.parentNode);
                            // console.log($(info.el.parentNode.cells[1].querySelector('.fc-cell-text')));
                            // console.log(info.el.querySelector('.fc-cell-text'));
                            //var questionMark = document.createElement('input');
                            //questionMark.type = 'checkbox';
                            //questionMark.id = "select_" + info.resource.id;
                            //questionMark.className = "checkchange";
                            //questionMark.name = "select_" + info.resource.id;
                            //// questionMark.checked = true;
                            //$(questionMark).attr('checked', info.select);



                            //var questionMark2 = document.createElement('input');
                            //questionMark2.type = 'checkbox';
                            //questionMark2.className = "checkchange";
                            //questionMark2.id = "leader_" + info.resource.id;
                            //questionMark2.name = "leader_" + info.resource.id;
                            //$(questionMark2).attr('checked', info.leader);

                            //var questionMark3 = document.createElement('input');
                            //questionMark3.type = 'checkbox';
                            //questionMark3.id = "fitter_" + info.resource.id;
                            //questionMark3.className = "checkchange";
                            //questionMark3.name = "fitter_" + info.resource.id;
                            //$(questionMark3).attr('checked', info.fitter);


                            //$(info.el.parentNode.cells[1].querySelector('.fc-cell-text')
                            //).html(questionMark);
                            //$(info.el.parentNode.cells[2].querySelector('.fc-cell-text')
                            //).html(questionMark2);
                            //$(info.el.parentNode.cells[3].querySelector('.fc-cell-text')
                            //).html(questionMark3);




                        },
                        resourceOrder: 'employeeLastName',
                        resources: resource,
                        events: evetns,
                        eventRender: function (info) {
                            //  console.log(info);
                            // console.log(info.event.start, info.event.end);
                            $(info.el).tooltip({ title: info.event.title, start: info.event.start, end: info.event.end });

                            // var eventContent = '<div class="fc-title">' + info.event.title + '</div>' +
                            //   '<div class="fc-tooltip-data" data-start="' + info.event.start + '" data-end="' + info.event.end + '">Hover for Details</div>';

                            // Set the event's HTML content
                            // $(info.el).tooltip(eventContent);


                            // var eventContent = '<div class="fc-title">' + info.event.title + '</div>' +
                            //    '<div class="fc-time">' + info.event.start + ' - ' + info.event.end + '</div>';

                            // Set the event's HTML content
                            // $(info.el).html(eventContent);

                            // console.log(event.title);
                            // Access custom properties and use them to customize the event rendering
                            // event.el.querySelector('.fc-title').append('<br>' + event.title);
                        },
                        eventClick: function (info) {

                            // console.log(info);
                            // console.log(info.event.start, info.event.end);
                            // console.log(info.event.id, info.event.title);
                            // console.log(info.event.extendedProps.eventType);
                            console.log(info.event.extendedProps.shiftId);
                            if (info.event.extendedProps.eventType == "Shift") {


                                objCommon.AjaxCall("/Home/GetShiftDataById", $.param({ shiftId: info.event.extendedProps.shiftId }), "GET", true, function (dresponse) {
                                    console.log(dresponse);
                                    dthis.RosterShift = {};
                                    dthis.RosterShift.DriverName = info.event.extendedProps.employeeName;
                                    dthis.RosterShift.ShiftLocation = dresponse.WorksiteDetails;
                                    dthis.RosterShift.Description = "";

                                    const dateObject = new Date(dresponse.StartDateTime);
                                    const finishdateObject = new Date(dresponse.FinishDateTime);
                                    if (!isNaN(dateObject)) {
                                        dthis.RosterShift.StartDateTime = moment(dateObject).format("DD-MM-YYYY HH:MM");
                                        dthis.RosterShift.FinishDateTime = moment(finishdateObject).format("DD-MM-YYYY HH:MM");
                                        //    // Format the Date object to a string in "YYYY-MM-DD" format
                                        //    //  data.StartDateTime = dateObject.toISOString().split('T')[0];

                                    }

                                    dthis.RosterShift.ShiftId = info.event.extendedProps.shiftId;

                                    //dxthis.RosterShift.FromActuall = dresponse.FirstName;
                                    // dxthis.RosterShift.ToActuall = dresponse.FirstName;

                                    $("#shiftModel").modal("show");
                                })

                            }
                            else {
                                console.log(info.event.extendedProps);
                                var fday = ("0" + info.event.start.getDate()).slice(-2);
                                var fmonth = ("0" + (info.event.start.getMonth() + 1)).slice(-2);

                                var tday = ("0" + info.event.end.getDate()).slice(-2);
                                var tmonth = ("0" + (info.event.end.getMonth() + 1)).slice(-2);

                                var ftoday = (fday) + "-" + (fmonth) + "-" + info.event.start.getFullYear();
                                var ttoday = (tday) + "-" + (tmonth) + "-" + info.event.end.getFullYear();
                                dthis.Absance.FromDate = ftoday;
                                dthis.Absance.ToDate = ttoday;
                                dthis.Absance.LeaveType = info.event.extendedProps.reason;
                                dthis.Absance.Reason = info.event.title;
                                $("#absanceModel").modal("show");
                            }
                            // console.log(jsEvent);
                            // console.log(view);
                        }
                        , dayRender: function (date) {

                            // Check if the day is Saturday (6) or Sunday (0)
                            //if (date.getDay() === 6 || date.getDay() === 0) {

                            var dayName = new Date(date.date).toLocaleDateString('en-US', { weekday: 'long' });
                            if (dayName == "Saturday" || dayName == "Sunday")
                                date.el.style.backgroundColor = '#f1e0e0'; // Change the background color
                            //  cell.css('color', 'white'); // Change the text color
                            // }
                        }
                    });


                    calendar.render();

                })
            },
            GetAbsance(data) {
                var dxthis = this.Absance;
                var json = JSON.parse(data);
                //console.log(json);
                dxthis.EmployeeId = json.id;


                var datadb = {
                    PageIndex: 1,
                    PageSize: 100,
                    SortColumn: "DateFrom",
                    SortOrder: "desc",
                    SearchText: "text",
                    EmployeeId: dxthis.EmployeeId
                }

                console.log("checkabse",datadb);
                objCommon.AjaxCall("/Abs/GetData", $.param(datadb), "GET", true, function (response) {

                    console.log(response);
                    var columnNames = [
                        { dataField: 'DateFromString', caption: 'Date From', dataType: 'date' },
                        { dataField: 'DateToString', caption: 'Date To', dataType: 'date' },
                        { dataField: 'LeaveType', caption: 'Type', dataType: 'string' }, { dataField: 'Reason', caption: 'Description', dataType: 'string' }];
                    const employeesStore = new DevExpress.data.ArrayStore({
                        key: 'Id',
                        data: response,
                    });

                    const dataGrid = $('#gridAbsance').dxDataGrid({

                        dataSource: employeesStore,
                        showBorders: true,
                        columns: columnNames,
                        loadPanel: {
                            enabled: true,
                        },
                        scrolling: {
                            columnRenderingMode: 'virtual',
                        },
                        columnsAutoWidth: true,
                        focusedRowEnabled: true,
                        paging: {
                            enabled: false,
                        },
                        editing: {
                            mode: 'cell',
                        },

                        //columns: ListCol,
                        sorting: {
                            mode: "multiple"
                        },
                        allowColumnResizing: true,
                        onRowClick: function (e) {
                            //console.log(e.data);
                            dxthis.AbsId = e.data.Id;
                            // dxthis.Edit(e.data);
                            //console.log(e.data);
                        },
                        rowAlternationEnabled: true,
                        onEditingStart() {
                            //console.log('EditingStart');
                        },

                        onRowRemoving: function (e) {

                        },
                        onContentReady(e) {
                            e.component.option('loadPanel.enabled', false);
                        },
                        onSelectionChanged(data) {
                            dataGrid.option('toolbar.items[1].options.disabled', !data.selectedRowsData.length);
                        },
                    }).dxDataGrid('instance');

                });
            },
            CloseShiftPopup() {
                $("#shiftModel").modal("hide");
            },
            CloseAbsancePopup() {
                $("#absanceModel").modal("hide");
            },
            OpenCommentPopup() {
                $("#commentModel").modal("show");
            },
            CloseCommentPopup() {
                $("#commentModel").modal("hide");
            },
            WeeklyComemntsPopup() {

                $("#WeeklyComments").modal("show");
            },
            WeeklyRosterReport() {
                window.location.href = "/Reports/Index?Type=PLANTCREWROSTERREPORT&ShiftId=0&FromWeek=" + $("#ddlFromWeek").val() + "&ToWeek=" + $("#ddlToWeek").val();
            },
            SubmitComment($event) {
                var et = this;
                if ($("#ddlFromWeek").val() == "" || $("#ddlFromWeek").val() == undefined || $("#ddlFromWeek").val() == null) {
                    objCommon.ShowMessage("Please select week.", "error");
                    return;
                }

                this.comment.WeekId = Number($("#ddlFromWeek").val());
                var param = this.comment;
                objCommon.AjaxCall("/Home/SaveWeeklyComments", JSON.stringify(param), "POST", true, function (response) {
                    if (response == "Saved") {

                        objCommon.ShowMessage("Added.", "success");
                    }
                    else {
                        objCommon.ShowMessage(response, "error");
                    }



                }, $event.currentTarget);
            },
            AddItems(dd) {

                this.comment.ListArrangements.push(this.item);
                this.item = {};

            },
            DeleteItem(item) {
                //   this.comment.CommentsData(item);

                this.comment.ListArrangements = $.grep(this.comment.ListArrangements, function (obj) {
                    return JSON.stringify(obj) !== JSON.stringify(item);
                });

            },
            SaveDescription($event) {
                var dxthis = this;
                var desData = {
                    ShiftId: dxthis.RosterShift.ShiftId,
                    RosterShiftDescription: dxthis.RosterShift.Description
                }
                objCommon.AjaxCall("/Roster/SaveShiftRosterDescription", JSON.stringify(desData), "POST", true, function (d) {
                    console.log(d);
                    if (d.Value == "success") {
                        objCommon.ShowMessage("Description Updated Successfully.", "success");
                        $("#shiftModel").modal("hide");
                    }
                    else {
                        objCommon.ShowMessage(d, "error");
                    }

                }, $event.currentTarget)
            },
            OpenShiftDetail() {       
                var dxthis = this;
                var shiftDetail = dxthis.ShiftData.filter(x => x.Id == dxthis.RosterShift.ShiftId)[0];
                dxthis.ActionType = "Update";
                dxthis.Edit(shiftDetail);
                        
            }
        },
        created() {
            this.GetData(null);


        },
        updated() {

        },
        mounted() {
            $('#rosterGrid').on('change', '.checkchange', function () {
                console.log($(this).attr("id"))
                if ($(this).is(':checked')) {
                    console.log('Checkbox is checked');
                } else {
                    console.log('Checkbox is unchecked');
                }
            });
        }
    })




})
