// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
  const toggleBtn = document.getElementById("sidebarCollapseBtn");
  if (toggleBtn) {
    toggleBtn.addEventListener("click", () => {
      const collapsed = document.documentElement.classList.toggle("sidebar-collapsed");
      try {
        localStorage.setItem("hr.sidebarCollapsed", collapsed ? "1" : "0");
      } catch (_) {}
    });
  }

  // Mobile: close offcanvas after clicking a nav link.
  document.addEventListener("click", (e) => {
    const link = e.target?.closest?.("a.app-nav-link");
    if (!link) return;

    const sidebarEl = document.getElementById("appSidebar");
    if (!sidebarEl) return;

    const offcanvas = window.bootstrap?.Offcanvas?.getInstance(sidebarEl);
    if (offcanvas) offcanvas.hide();
  });
})();

