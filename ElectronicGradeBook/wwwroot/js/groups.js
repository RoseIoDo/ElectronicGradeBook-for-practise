// Глобальна змінна для збереження останньої "академічної дати"
var currentAcademicDate = null;

$(document).ready(function () {
    console.log("groups.js loaded");

    loadGroups();

    // Обчислення року випуску та курсу при зміні року вступу або виборі спеціальності
    $("#enrollmentYear, #specialtySelect").on("change", function () {
        var enrollmentYear = parseInt($("#enrollmentYear").val());
        var duration = parseInt($("#specialtySelect option:selected").data("duration"));
        if (enrollmentYear && duration) {
            $("#graduationYear").val(enrollmentYear + duration);
        }
        if (enrollmentYear) {
            var currentYear = new Date().getFullYear();
            var studyYear = currentYear - enrollmentYear + 1;
            $("#currentStudyYear").val(studyYear);
        }
    });

    // Обробники для оновлення академічної дати
    $("#updateAcademicDateBtn").click(function () {
        var customDateStr = $("#academicDate").val();
        updateAcademicDate(customDateStr);
    });

    $("#resetAcademicDateBtn").click(function () {
        var todayStr = new Date().toISOString().split('T')[0];
        $("#academicDate").val(todayStr);
        updateAcademicDate(todayStr);
    });

    function updateAcademicDate(dateStr) {
        $.ajax({
            url: "/Groups/UpdateGroupsAcademicDate",
            type: "POST",
            data: { customDate: dateStr },
            success: function (resp) {
                $("#academicUpdateResult").html("<div class='alert alert-success'>" + resp.message + "</div>");
                // Зберігаємо нову академічну дату
                currentAcademicDate = dateStr ? new Date(dateStr) : new Date();
                loadGroups();
            },
            error: function () {
                $("#academicUpdateResult").html("<div class='alert alert-danger'>Помилка оновлення груп.</div>");
            }
        });
    }

    // Обробник для кнопки "Додати групу"
    $("#addGroupBtn").click(function () {
        clearForm();
        $("#groupModalLabel").text("Додати групу");
        $("#groupModal").modal("show");
    });

    // Обробник для редагування групи
    $(document).on("click", ".edit-btn", function () {
        clearForm();
        $("#groupModalLabel").text("Редагувати групу");

        $("#groupId").val($(this).data("id"));
        $("#groupPrefix").val($(this).data("groupprefix"));
        $("#groupNumber").val($(this).data("groupnumber"));
        $("#currentStudyYear").val($(this).data("currentstudyyear"));
        $("#enrollmentYear").val($(this).data("enrollmentyear"));
        $("#graduationYear").val($(this).data("graduationyear"));
        $("#specialtySelect").val($(this).data("specialtyid"));

        $("#groupModal").modal("show");
    });

    // Збереження групи (додавання або редагування)
    $("#saveGroupBtn").click(function () {
        var model = {
            id: parseInt($("#groupId").val()) || 0,
            groupPrefix: $("#groupPrefix").val().trim(),
            groupNumber: parseInt($("#groupNumber").val()) || 0,
            currentStudyYear: parseInt($("#currentStudyYear").val()) || 0,
            enrollmentYear: parseInt($("#enrollmentYear").val()) || 0,
            graduationYear: parseInt($("#graduationYear").val()) || 0,
            specialtyId: parseInt($("#specialtySelect").val()) || 0
        };

        if (!model.groupPrefix) {
            alert("Префікс не може бути порожнім.");
            return;
        }

        var url = model.id ? "/Groups/Edit" : "/Groups/Add";
        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(model),
            success: function (resp) {
                if (resp.success) {
                    $("#groupModal").modal("hide");
                    loadGroups();
                } else {
                    alert(resp.message);
                }
            },
            error: function () {
                alert("Помилка збереження групи.");
            }
        });
    });

    // Видалення групи
    $(document).on("click", ".delete-btn", function () {
        var id = $(this).data("id");
        if (confirm("Видалити групу?")) {
            $.ajax({
                url: `/Groups/Delete?id=${id}`,
                type: "POST",
                success: function (resp) {
                    if (resp.success) {
                        alert(resp.message);
                        loadGroups();
                    } else {
                        alert(resp.message);
                    }
                },
                error: function () {
                    alert("Помилка видалення групи.");
                }
            });
        }
    });

    // Функція завантаження груп із групуванням за категоріями
    function loadGroups() {
        $.ajax({
            url: "/Groups/GetAll",
            type: "GET",
            success: function (list) {
                var $tbody = $("#groupsTable tbody");
                $tbody.empty();
                if (list.length === 0) return;

                // Використовуємо академічну дату з currentAcademicDate, якщо встановлена, інакше поточну
                var now = currentAcademicDate ? currentAcademicDate : new Date();
                // Для академічного року: якщо місяць >= 8 (тобто вересень і пізніше, оскільки місяці 0-indexовані), academicYearStart = now.getFullYear(), інакше – now.getFullYear()-1.
                var academicYearStart = now.getMonth() >= 8 ? now.getFullYear() : now.getFullYear() - 1;

                // Розділяємо групи: актуальні (не випущені) та випущені
                var currentGroups = [];
                var graduatedGroups = [];
                list.forEach(function (g) {
                    // Для груп, де academicYearStart >= graduationYear, вважаємо їх випущеними
                    if (academicYearStart >= g.graduationYear) {
                        graduatedGroups.push(g);
                    } else {
                        currentGroups.push(g);
                    }
                });

                // Рендеринг актуальних груп за префіксом і курсом
                if (currentGroups.length > 0) {
                    $tbody.append("<tr class='table-primary'><td colspan='8'><strong>Актуальні групи</strong></td></tr>");
                    var groupsByPrefix = {};
                    currentGroups.forEach(function (g) {
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
                        var courses = Object.keys(groupsByCourse).sort(function (a, b) { return a - b; });
                        courses.forEach(function (course) {
                            var groupsInCourse = groupsByCourse[course].sort(function (a, b) { return a.groupNumber - b.groupNumber; });
                            // Шукаємо базову групу: де groupNumber === course
                            var baseGroup = groupsInCourse.find(function (g) {
                                return g.groupNumber === parseInt(course);
                            });
                            if (baseGroup) {
                                $tbody.append("<tr class='table-secondary'><td colspan='8'><strong>" + prefix + "-" + course + " (Загальна група)</strong></td></tr>");
                                groupsInCourse = groupsInCourse.filter(function (g) {
                                    return g.groupNumber !== parseInt(course);
                                });
                            } else {
                                $tbody.append("<tr class='table-secondary'><td colspan='8'><strong>" + prefix + "-" + course + "</strong></td></tr>");
                            }
                            groupsInCourse.forEach(function (g) {
                                $tbody.append(`
                                    <tr>
                                        <td>${g.id}</td>
                                        <td>${g.groupPrefix}</td>
                                        <td>${g.groupNumber}</td>
                                        <td>${g.currentStudyYear}</td>
                                        <td>${g.enrollmentYear}</td>
                                        <td>${g.graduationYear}</td>
                                        <td>${g.specialtyName || ""}</td>
                                        <td>
                                            <button class="btn btn-warning btn-sm edit-btn"
                                                data-id="${g.id}"
                                                data-groupprefix="${g.groupPrefix}"
                                                data-groupnumber="${g.groupNumber}"
                                                data-currentstudyyear="${g.currentStudyYear}"
                                                data-enrollmentyear="${g.enrollmentYear}"
                                                data-graduationyear="${g.graduationYear}"
                                                data-specialtyid="${g.specialtyId}">
                                                Редагувати
                                            </button>
                                            <button class="btn btn-danger btn-sm delete-btn" data-id="${g.id}">
                                                Видалити
                                            </button>
                                        </td>
                                    </tr>
                                `);
                            });
                        });
                    });
                }

                // Рендеринг випущених груп за префіксом і роками
                if (graduatedGroups.length > 0) {
                    $tbody.append("<tr class='table-danger'><td colspan='8'><strong>Випущені групи</strong></td></tr>");
                    var gradGroupsByPrefix = {};
                    graduatedGroups.forEach(function (g) {
                        if (!gradGroupsByPrefix[g.groupPrefix]) {
                            gradGroupsByPrefix[g.groupPrefix] = [];
                        }
                        gradGroupsByPrefix[g.groupPrefix].push(g);
                    });
                    Object.keys(gradGroupsByPrefix).forEach(function (prefix) {
                        var groupsForPrefix = gradGroupsByPrefix[prefix];
                        // Групуємо за парою "EnrollmentYear-GraduationYear"
                        var groupsByYearPair = {};
                        groupsForPrefix.forEach(function (g) {
                            var key = g.enrollmentYear + "-" + g.graduationYear;
                            if (!groupsByYearPair[key]) {
                                groupsByYearPair[key] = [];
                            }
                            groupsByYearPair[key].push(g);
                        });
                        var yearPairs = Object.keys(groupsByYearPair).sort(function (a, b) {
                            var aYear = parseInt(a.split("-")[0]);
                            var bYear = parseInt(b.split("-")[0]);
                            return aYear - bYear;
                        });
                        yearPairs.forEach(function (yearPair) {
                            $tbody.append("<tr class='table-secondary'><td colspan='8'><strong>" + prefix + " - " + yearPair + " (Випущені)</strong></td></tr>");
                            var subgroupList = groupsByYearPair[yearPair].sort(function (a, b) {
                                return a.groupNumber - b.groupNumber;
                            });
                            subgroupList.forEach(function (g) {
                                $tbody.append(`
                                    <tr>
                                        <td>${g.id}</td>
                                        <td>${g.groupPrefix}</td>
                                        <td>${g.groupNumber}</td>
                                        <td>${g.currentStudyYear}</td>
                                        <td>${g.enrollmentYear}</td>
                                        <td>${g.graduationYear}</td>
                                        <td>${g.specialtyName || ""}</td>
                                        <td>
                                            <button class="btn btn-warning btn-sm edit-btn"
                                                data-id="${g.id}"
                                                data-groupprefix="${g.groupPrefix}"
                                                data-groupnumber="${g.groupNumber}"
                                                data-currentstudyyear="${g.currentStudyYear}"
                                                data-enrollmentyear="${g.enrollmentYear}"
                                                data-graduationyear="${g.graduationYear}"
                                                data-specialtyid="${g.specialtyId}">
                                                Редагувати
                                            </button>
                                            <button class="btn btn-danger btn-sm delete-btn" data-id="${g.id}">
                                                Видалити
                                            </button>
                                        </td>
                                    </tr>
                                `);
                            });
                        });
                    });
                }
            },
            error: function () {
                alert("Помилка завантаження груп.");
            }
        });
    }

    // Функція очищення форми
    function clearForm() {
        $("#groupId").val("");
        $("#groupPrefix").val("");
        $("#groupNumber").val("");
        $("#currentStudyYear").val("");
        $("#enrollmentYear").val("");
        $("#graduationYear").val("");
        $("#specialtySelect").val("");
    }
});
