import { Routes, Route } from "react-router-dom";

// Auth Pages
import Register from "../pages/Register";
import Login from "../pages/Login";
import ForgotPassword from "../pages/ForgotPassword";
import VerifyOTP from "../pages/VerifyOTP";
import ResetPassword from "../pages/ResetPassword";

// Dashboard Layout
import DashboardLayout from "../Dashboard/DashboardLayout";

// Subject Management
import SubjectList from "../Dashboard/Subject Management/SubjectList";
import AddSubject from "../Dashboard/Subject Management/AddSubject";

// Staff Management
import StaffList from "../Dashboard/Staff Management/StaffList";
import StaffSubjectAllocation from "../Dashboard/Staff Management/StaffSubjectAllocation";

function AppRoutes() {
  return (
    <Routes>

      {/* Authentication Routes */}
      <Route path="/" element={<Login />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/verify-otp" element={<VerifyOTP />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      {/* Dashboard Layout */}
      <Route path="/dashboard" element={<DashboardLayout />}>

        <Route index element={<SubjectList />} />

        <Route path="subjects" element={<SubjectList />} />
        <Route path="subjects/add" element={<AddSubject />} />

        {/* Staff Management Routes */}
        <Route path="staff" element={<StaffList />} />
        <Route path="staff/subject-allocation" element={<StaffSubjectAllocation />} />

        {/* Backward compatibility aliases */}
        <Route path="faculty" element={<StaffList />} />
        <Route path="faculty/subject-allocation" element={<StaffSubjectAllocation />} />

      </Route>

    </Routes>
  );
}


export default AppRoutes;