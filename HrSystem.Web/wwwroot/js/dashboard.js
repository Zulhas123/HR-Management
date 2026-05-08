(function () {
  const getText = (value) => (value === null || value === undefined ? "" : String(value));

  async function refreshDashboard() {
    try {
      const resp = await fetch("/api/dashboard/summary", { credentials: "same-origin", headers: { "Accept": "application/json" } });
      if (!resp.ok) return;

      const data = await resp.json();

      const byId = (id) => document.getElementById(id);
      const set = (id, value) => {
        const el = byId(id);
        if (el) el.textContent = getText(value);
      };

      set("metricTotalEmployees", data.totalEmployees);
      set("metricAttendanceToday", data.attendanceToday);
      set("metricPendingLeaveRequests", data.pendingLeaveRequests);
      set("metricPendingOvertimeRequests", data.pendingOvertimeRequests);
      set("metricOpenJobPostings", data.openJobPostings);

      const updatedAt = byId("metricUpdatedAt");
      if (updatedAt && data.generatedAtUtc) {
        const d = new Date(data.generatedAtUtc);
        if (!Number.isNaN(d.getTime())) {
          updatedAt.textContent = "Updated: " + d.toLocaleString();
        }
      }
    } catch {
      // ignore transient errors
    }
  }

  document.addEventListener("DOMContentLoaded", () => {
    const refreshSeconds = (window.hrDashboard && window.hrDashboard.refreshSeconds) || 30;
    const intervalMs = Math.max(5000, Math.min(300000, refreshSeconds * 1000));

    refreshDashboard();
    window.setInterval(refreshDashboard, intervalMs);
  });
})();
