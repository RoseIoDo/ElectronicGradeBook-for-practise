$(document).ready(function () {
    console.log("gradebook.js loaded");

    const $facultySelect = $("#facultySelect");
    const $specialtySelect = $("#specialtySelect");
    const $groupSelect = $("#groupSelect");
    const $loadSubjectsBtn = $("#loadSubjectsBtn");

    const $tabs = $("#subjectsTabs");
    const $tabContent = $("#subjectsTabContent");

    // 1) Завантажуємо факультети
    loadFaculties();

    // Зміна факультету => specialties
    $facultySelect.change(function () {
        const facId = parseInt($facultySelect.val()) || 0;
        loadSpecialties(facId);
    });

    // Зміна спеціальності => groups/courses
    $specialtySelect.change(function () {
        const specId = parseInt($specialtySelect.val()) || 0;
        loadGroups(specId);
    });

    // Клік “Завантажити предмети”
    $loadSubjectsBtn.click(function () {
        loadSubjectOfferings();
    });

    // ===========================

    function loadFaculties() {
        $.ajax({
            url: "/GradeBook/GetFaculties",
            type: "GET",
            success: function (data) {
                $facultySelect.empty();
                $facultySelect.append(`<option value="0">-- Факультет --</option>`);
                if (data && data.length > 0) {
                    data.forEach(f => {
                        $facultySelect.append(`<option value="${f.id}">${f.name}</option>`);
                    });
                }
            }
        });
    }

    function loadSpecialties(facId) {
        if (facId <= 0) {
            $specialtySelect.empty().append(`<option value="0">-- Спеціальність --</option>`);
            $groupSelect.empty().append(`<option value="">-- Група --</option>`);
            return;
        }
        $.ajax({
            url: `/GradeBook/GetSpecialties?facultyId=${facId}`,
            type: "GET",
            success: function (list) {
                $specialtySelect.empty();
                $specialtySelect.append(`<option value="0">-- Спеціальність --</option>`);
                if (list && list.length > 0) {
                    list.forEach(s => {
                        $specialtySelect.append(`<option value="${s.id}">${s.name}</option>`);
                    });
                }
                $groupSelect.empty().append(`<option value="">-- Група --</option>`);
            }
        });
    }

    function loadGroups(specId) {
        if (specId <= 0) {
            $groupSelect.empty().append(`<option value="">-- Група --</option>`);
            return;
        }
        $.ajax({
            url: `/GradeBook/GetGroups?specialtyId=${specId}`,
            type: "GET",
            success: function (groups) {
                $groupSelect.empty();
                $groupSelect.append(`<option value="">(Виберіть або course-...)</option>`);
                if (groups && groups.length > 0) {
                    const map = {};
                    groups.forEach(g => {
                        const key = g.groupPrefix + "-" + g.currentStudyYear;
                        if (!map[key]) map[key] = [];
                        map[key].push(g);
                    });
                    Object.keys(map).forEach(key => {
                        $groupSelect.append(`<option value="course-${key}">${key} (весь курс)</option>`);
                        map[key].forEach(gr => {
                            const label = `${gr.groupPrefix}-${gr.groupNumber} (ID=${gr.id})`;
                            $groupSelect.append(`<option value="${gr.id}">   ${label}</option>`);
                        });
                    });
                }
            }
        });
    }

    function loadSubjectOfferings() {
        const groupVal = $groupSelect.val() || "";
        if (!groupVal) {
            alert("Оберіть групу або курс!");
            return;
        }
        $.ajax({
            url: `/GradeBook/GetSubjectOfferings?groupVal=${groupVal}`,
            type: "GET",
            success: function (offerings) {
                if (!offerings || offerings.length === 0) {
                    alert("Немає предметів для обраної групи/курсу.");
                    $tabs.html("");
                    $tabContent.html("");
                    return;
                }
                renderTabs(offerings);
            },
            error: function () {
                alert("Помилка завантаження предметів.");
            }
        });
    }

    function renderTabs(offerings) {
        $tabs.html("");
        $tabContent.html("");

        offerings.forEach((off, idx) => {
            const offId = off.id;
            const tabId = "offTab_" + offId;
            const activeClass = (idx === 0) ? "active" : "";
            const ariaSel = (idx === 0) ? "true" : "false";

            $tabs.append(`
              <li class="nav-item">
                <button class="nav-link ${activeClass}"
                        id="${tabId}-tab"
                        data-bs-toggle="tab"
                        data-bs-target="#${tabId}"
                        type="button" role="tab"
                        aria-controls="${tabId}"
                        aria-selected="${ariaSel}">
                  ${off.subjectName}
                </button>
              </li>
            `);

            const activePane = (idx === 0) ? "show active" : "";
            $tabContent.append(`
              <div class="tab-pane fade ${activePane}"
                   id="${tabId}"
                   role="tabpanel"
                   aria-labelledby="${tabId}-tab">
                <div id="tableContainer_${offId}"></div>
              </div>
            `);
        });

        if (offerings.length > 0) {
            loadGradeTable(offerings[0].id);
        }

        $tabs.find(".nav-link").on("shown.bs.tab", function (e) {
            const target = $(e.target).attr("data-bs-target");
            const offId = parseInt(target.replace("#offTab_", ""));
            loadGradeTable(offId);
        });
    }

    // Завантажити оцінки для 1 SubjectOffering
    function loadGradeTable(offId) {
        $.ajax({
            url: `/GradeBook/GetGradesForOffering?offId=${offId}`,
            type: "GET",
            success: function (rows) {
                const $cont = $(`#tableContainer_${offId}`);
                if (!rows || rows.length === 0) {
                    $cont.html("<p>Немає студентів чи оцінок.</p>");
                    return;
                }
                renderGradeTable(offId, rows, $cont);
            },
            error: function () {
                alert("Помилка завантаження оцінок.");
            }
        });
    }

    // Малюємо таблицю, додаємо кнопку "Історія"
    function renderGradeTable(offId, rows, $container) {
        let html = `
          <table class="table table-custom">
            <thead>
              <tr>
                <th>Студент</th>
                <th>Оцінка</th>
                <th>Перездача</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
        `;
        rows.forEach(r => {
            const val = (r.points != null) ? r.points : "";
            const chk = r.isRetake ? "checked" : "";
            html += `
              <tr>
                <td>${r.studentName}</td>
                <td>
                  <input type="number" step="0.1" class="form-control form-control-sm grade-input"
                         data-off="${offId}" data-stud="${r.studentId}"
                         value="${val}">
                </td>
                <td>
                  <input type="checkbox" class="form-check-input retake-chk"
                         data-off="${offId}" data-stud="${r.studentId}"
                         ${chk}>
                </td>
                <td>
                  <button class="btn btn-sm btn-secondary history-btn"
                          data-off="${offId}" data-stud="${r.studentId}">
                    Історія
                  </button>
                </td>
              </tr>
            `;
        });
        html += `</tbody></table>`;
        $container.html(html);

        // Прив’язуємо обробники
        $container.find(".grade-input").change(function () {
            updateGrade($(this));
        });
        $container.find(".retake-chk").change(function () {
            updateGrade($(this));
        });
        $container.find(".history-btn").click(function () {
            const off = $(this).data("off");
            const st = $(this).data("stud");
            showHistory(off, st);
        });
    }

    function updateGrade($el) {
        const offId = $el.data("off");
        const studId = $el.data("stud");
        const pointsVal = parseFloat(
            $(`.grade-input[data-off='${offId}'][data-stud='${studId}']`).val()
        ) || null;
        const retakeVal = $(`.retake-chk[data-off='${offId}'][data-stud='${studId}']`).is(":checked");

        $.ajax({
            url: "/GradeBook/UpdateGrade",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                subjectOfferingId: offId,
                studentId: studId,
                points: pointsVal,
                isRetake: retakeVal
            }),
            success: function (resp) {
                if (!resp.success) {
                    alert("Помилка: " + resp.message);
                }
            },
            error: function () {
                alert("Помилка оновлення оцінки.");
            }
        });
    }

    // Показати історію (модальне вікно)
    function showHistory(offId, studId) {
        $.ajax({
            url: `/GradeBook/GetGradeHistory?offId=${offId}&studentId=${studId}`,
            type: "GET",
            success: function (list) {
                // list = [{points, isRetake, timestamp}, ...]
                if (!list || list.length === 0) {
                    $("#historyBody").html("<p>Історія порожня.</p>");
                } else {
                    let hHtml = `<ul class="list-group">`;
                    list.forEach(v => {
                        hHtml += `
                          <li class="list-group-item">
                            <strong>${v.points ?? "—"}</strong>
                            (перездача = ${v.isRetake}),
                            <em>${v.timestamp}</em>
                          </li>
                        `;
                    });
                    hHtml += `</ul>`;
                    $("#historyBody").html(hHtml);
                }
                $("#historyModal").modal("show");
            },
            error: function () {
                alert("Помилка завантаження історії.");
            }
        });
    }
});
