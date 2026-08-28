import { Outlet, NavLink } from "react-router-dom";
import "./Dashboard.css";

function DashboardLayout() {
  return (
    <div className="dashboard-layout">

      {/* Sidebar */}
      <aside className="dashboard-sidebar">

        <div className="sidebar-logo">
          CMS
        </div>

        <nav className="sidebar-menu">

          <NavLink to="/dashboard" end>
            Dashboard
          </NavLink>

          <div className="sidebar-title">
            Academic Management
          </div>

          <NavLink to="/dashboard/subjects">
            Subjects
          </NavLink>

          <NavLink to="/dashboard/subjects/add">
            Add Subject
          </NavLink>

          <div className="sidebar-title">
            Staff Management
          </div>

          <NavLink to="/dashboard/staff">
            Staff List
          </NavLink>

          <NavLink to="/dashboard/staff/subject-allocation">
            Staff Subject Allocation
          </NavLink>

          <div className="sidebar-title">
            Certificates
          </div>

          <NavLink to="/dashboard/certificates">
            Certificate Management
          </NavLink>

          <div className="sidebar-title">
            Reports &amp; Analytics
          </div>

          <NavLink to="/dashboard/reports">
            Reports &amp; Analytics
          </NavLink>

        </nav>
      </aside>

      {/* Main Content */}
      <div className="dashboard-main">


        <main className="dashboard-content">
          <Outlet />
        </main>

      </div>

    </div>
  );
}

export default DashboardLayout;