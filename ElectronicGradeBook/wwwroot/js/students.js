$(document).ready(function () {
    console.log("students.js loaded");

    loadStudents();
    loadGroupsForDropdown();

    $("#addStudentBtn").click(function () {
        clearStudentForm();
        $("#studentModalLabel").text("Додати студента");
        $("#studentModal").modal("show");
    });

    $("#saveStudentBtn").click(function () {
        var selectedOption = $("#groupSelect option:selected");
        var enrollmentDate = selectedOption.data("enrollment") || "";
        var graduationDate = selectedOption.data("graduation") || "";

        var model = {
            id: parseInt($("#studentId").val()) || 0,
            fullName: $("#fullName").val().trim(),
            enrollmentDate: enrollmentDate,
            graduationDate: graduationDate,
            isActive: $("#isActive").is(":checked"),
            groupId: parseInt($("#groupSelect").val()) || 0
        };

        if (!model.fullName) {
            alert("ПІБ не може бути порожнім.");
            return;
        }
        if (!model.groupId) {
            alert("Оберіть групу.");
            return;
        }
        var url = model.id ? "/Students/Edit" : "/Students/Add";
        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(model),
            success: function (resp) {
                if (resp.success) {
                    $("#studentModal").modal("hide");
                    loadStudents();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження студента.");
            }
        });
    });

    $(document).on("click", ".delete-student-btn", function () {
        var id = $(this).data("id");
        if (confirm("Видалити студента?")) {
            $.ajax({
                url: `/Students/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadStudents();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення студента.");
                }
            });
        }
    });

    // Функція завантаження студентів із розмежуванням за групами
    function loadStudents() {
        $.ajax({
            url: "/Students/GetAll",
            type: "GET",
            success: function (list) {
                var $tbody = $("#studentsTable tbody");
                $tbody.empty();
                if (list.length === 0) return;

                // Використовуємо глобальну змінну currentAcademicDate (якщо встановлена), інакше поточну дату
                var now = (typeof currentAcademicDate !== "undefined" && currentAcademicDate) ? new Date(currentAcademicDate) : new Date();
                var academicYearStart = now.getMonth() >= 8 ? now.getFullYear() : now.getFullYear() - 1;

                // Розділяємо студентів: активні (де academicYearStart < groupGraduationYear) та випущені (де academicYearStart >= groupGraduationYear)
                var activeStudents = [];
                var graduatedStudents = [];
                list.forEach(function (s) {
                    if (academicYearStart >= s.groupGraduationYear) {
                        graduatedStudents.push(s);
                    } else {
                        activeStudents.push(s);
                    }
                });

                // Функція для рендерингу підгрупи студентів із header-рядком (використовує colspan="7")
                function renderStudentGroup(headerText, students) {
                    $tbody.append("<tr class='table-secondary'><td colspan='7'><strong>" + headerText + "</strong></td></tr>");
                    // Групуємо студентів за groupId, щоб один header не повторювався
                    var studentsByGroup = {};
                    students.forEach(function (s) {
                        var key = s.groupId;
                        if (!studentsByGroup[key]) {
                            studentsByGroup[key] = [];
                        }
                        studentsByGroup[key].push(s);
                    });
                    Object.keys(studentsByGroup).forEach(function (groupId) {
                        var groupStudents = studentsByGroup[groupId];
                        var hdr = groupStudents[0];
                        var groupHeader = "";
                        if (headerText.indexOf("Випущені") !== -1) {
                            groupHeader = hdr.groupPrefix + " (" + hdr.groupEnrollmentYear + " - " + hdr.groupGraduationYear + ")";
                        } else {
                            groupHeader = (hdr.groupNumber === hdr.groupCurrentStudyYear) ?
                                (hdr.groupPrefix + "-" + hdr.groupCurrentStudyYear + " (Загальна група)") :
                                (hdr.groupPrefix + "-" + hdr.groupNumber);
                        }
                        $tbody.append("<tr class='table-info'><td colspan='7'>" + groupHeader + "</td></tr>");
                        groupStudents.forEach(function (s) {
                            $tbody.append(`
                                <tr>
                                    <td>${s.id}</td>
                                    <td>${s.fullName}</td>
                                    <td>${s.enrollmentDate ? s.enrollmentDate.split("T")[0] : ""}</td>
                                    <td>${s.graduationDate ? s.graduationDate.split("T")[0] : ""}</td>
                                    <td>${s.isActive ? "Так" : "Ні"}</td>
                                    <td>${s.groupPrefix + "-" + s.groupNumber}</td>
                                    <td>
                                        <button class="btn btn-warning btn-sm edit-student-btn"
                                            data-id="${s.id}"
                                            data-fullname="${s.fullName}"
                                            data-enrollmentdate="${s.enrollmentDate ? s.enrollmentDate.split("T")[0] : ""}"
                                            data-graduationdate="${s.graduationDate ? s.graduationDate.split("T")[0] : ""}"
                                            data-isactive="${s.isActive}"
                                            data-groupid="${s.groupId}">
                                            Редагувати
                                        </button>
                                        <button class="btn btn-danger btn-sm delete-student-btn" data-id="${s.id}">
                                            Видалити
                                        </button>
                                    </td>
                                </tr>
                            `);
                        });
                    });
                }

                if (activeStudents.length > 0) {
                    $tbody.append("<tr class='table-primary'><td colspan='7'><strong>Актуальні групи</strong></td></tr>");
                    var activeByPrefix = {};
                    activeStudents.forEach(function (s) {
                        if (!activeByPrefix[s.groupPrefix]) {
                            activeByPrefix[s.groupPrefix] = [];
                        }
                        activeByPrefix[s.groupPrefix].push(s);
                    });
                    Object.keys(activeByPrefix).forEach(function (prefix) {
                        var groupList = activeByPrefix[prefix];
                        var byCourse = {};
                        groupList.forEach(function (s) {
                            var course = s.groupCurrentStudyYear;
                            if (!byCourse[course]) {
                                byCourse[course] = [];
                            }
                            byCourse[course].push(s);
                        });
                        Object.keys(byCourse).sort(function (a, b) { return a - b; }).forEach(function (course) {
                            renderStudentGroup(prefix + "-" + course, byCourse[course]);
                        });
                    });
                }

                if (graduatedStudents.length > 0) {
                    $tbody.append("<tr class='table-danger'><td colspan='7'><strong>Випущені групи</strong></td></tr>");
                    var gradByPrefix = {};
                    graduatedStudents.forEach(function (s) {
                        if (!gradByPrefix[s.groupPrefix]) {
                            gradByPrefix[s.groupPrefix] = [];
                        }
                        gradByPrefix[s.groupPrefix].push(s);
                    });
                    Object.keys(gradByPrefix).forEach(function (prefix) {
                        var groupList = gradByPrefix[prefix];
                        var byYearPair = {};
                        groupList.forEach(function (s) {
                            var key = s.groupEnrollmentYear + "-" + s.groupGraduationYear;
                            if (!byYearPair[key]) {
                                byYearPair[key] = [];
                            }
                            byYearPair[key].push(s);
                        });
                        Object.keys(byYearPair).sort(function (a, b) {
                            var aYear = parseInt(a.split("-")[0]);
                            var bYear = parseInt(b.split("-")[0]);
                            return aYear - bYear;
                        }).forEach(function (yearPair) {
                            renderStudentGroup(prefix + " (" + yearPair + ") (Випущені)", byYearPair[yearPair]);
                        });
                    });
                }
            },
            error: function () {
                alert("Помилка завантаження студентів.");
            }
        });
    }

    function loadGroupsForDropdown() {
        $.ajax({
            url: "/Groups/GetAll",
            type: "GET",
            success: function (list) {
                var $select = $("#groupSelect");
                $select.empty();
                if (list.length === 0) {
                    $select.append("<option value=''>Немає доступних груп</option>");
                    return;
                }

                var now = (typeof currentAcademicDate !== "undefined" && currentAcademicDate) ? new Date(currentAcademicDate) : new Date();
                var academicYearStart = now.getMonth() >= 8 ? now.getFullYear() : now.getFullYear() - 1;

                var activeGroups = [];
                var graduatedGroups = [];
                list.forEach(function (g) {
                    if (academicYearStart >= g.graduationYear) {
                        graduatedGroups.push(g);
                    } else {
                        activeGroups.push(g);
                    }
                });

                if (activeGroups.length > 0) {
                    $select.append("<option disabled style='font-weight:bold;'>Актуальні групи</option>");

                    var groupsByPrefix = {};
                    activeGroups.forEach(function (g) {
                        if (!groupsByPrefix[g.groupPrefix]) {
                            groupsByPrefix[g.groupPrefix] = [];
                        }
                        groupsByPrefix[g.groupPrefix].push(g);
                    });
                    Object.keys(groupsByPrefix).forEach(function (prefix) {
                        var prefixGroups = groupsByPrefix[prefix];
                        var groupsByCourse = {};
                        prefixGroups.forEach(function (g) {
                            var course = g.currentStudyYear;
                            if (!groupsByCourse[course]) {
                                groupsByCourse[course] = [];
                            }
                            groupsByCourse[course].push(g);
                        });
                        Object.keys(groupsByCourse).sort(function (a, b) { return a - b; }).forEach(function (course) {
                            var groupsInCourse = groupsByCourse[course].sort(function (a, b) { return a.groupNumber - b.groupNumber; });
                            var baseGroup = groupsInCourse.find(function (g) {
                                return g.groupNumber === parseInt(course);
                            });
                            var headerText = "";
                            if (baseGroup) {
                                headerText = prefix + "-" + course + " (Загальна група)";
                                groupsInCourse = groupsInCourse.filter(function (g) {
                                    return g.groupNumber !== parseInt(course);
                                });
                            } else {
                                headerText = prefix + "-" + course;
                            }
                            $select.append("<option disabled style='font-weight:bold;'>" + headerText + "</option>");
                            groupsInCourse.forEach(function (g) {
                                var optionText = g.groupPrefix + "-" + g.groupNumber;
                                var $option = $("<option></option>").val(g.id).text("   " + optionText);
                                $option.attr("data-enrollment", g.enrollmentYear + "-09-01");
                                $option.attr("data-graduation", g.graduationYear + "-06-30");
                                $select.append($option);
                            });
                        });
                    });
                }

                if (graduatedGroups.length > 0) {
                    $select.append("<option disabled style='font-weight:bold;'>Випущені групи</option>");
                    var gradGroupsByPrefix = {};
                    graduatedGroups.forEach(function (g) {
                        if (!gradGroupsByPrefix[g.groupPrefix]) {
                            gradGroupsByPrefix[g.groupPrefix] = [];
                        }
                        gradGroupsByPrefix[g.groupPrefix].push(g);
                    });
                    Object.keys(gradGroupsByPrefix).forEach(function (prefix) {
                        var groupsForPrefix = gradGroupsByPrefix[prefix];
                        var groupsByYearPair = {};
                        groupsForPrefix.forEach(function (g) {
                            var enroll = g.enrollmentYear != null ? g.enrollmentYear : "N/A";
                            var grad = g.graduationYear != null ? g.graduationYear : "N/A";
                            var key = enroll + "-" + grad;
                            if (!groupsByYearPair[key]) {
                                groupsByYearPair[key] = [];
                            }
                            groupsByYearPair[key].push(g);
                        });
                        Object.keys(groupsByYearPair).sort(function (a, b) {
                            var aYear = parseInt(a.split("-")[0]);
                            var bYear = parseInt(b.split("-")[0]);
                            return aYear - bYear;
                        }).forEach(function (yearPair) {
                            var headerText = prefix + " (" + yearPair + ") (Випущені)";
                            $select.append("<option disabled style='font-weight:bold;'>" + headerText + "</option>");
                            var subgroupList = groupsByYearPair[yearPair].sort(function (a, b) { return a.groupNumber - b.groupNumber; });
                            subgroupList.forEach(function (g) {
                                var optionText = g.groupPrefix + "-" + g.groupNumber;
                                var $option = $("<option></option>").val(g.id).text("   " + optionText);
                                $option.attr("data-enrollment", g.enrollmentYear + "-09-01");
                                $option.attr("data-graduation", g.graduationYear + "-06-30");
                                $select.append($option);
                            });
                        });
                    });
                }
            },
            error: function () {
                alert("Помилка завантаження груп для вибору.");
            }
        });
    }

    function clearStudentForm() {
        $("#studentId").val("");
        $("#fullName").val("");
        $("#isActive").prop("checked", true);
        $("#groupSelect").val("");
    }
});
