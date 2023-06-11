import { useLocation, Navigate, Outlet } from "react-router-dom";
import useAuth from "../hooks/useAuth";

const RequireAuth = ({ allowedRoles: allowedRoles }) => {
  const { auth } = useAuth();
  const location = useLocation();
  console.log(auth);

  // return allowedRoles?.includes(auth?.role) ? (
  //   <Outlet />
  // ) : auth?.user ? (
  //   <Navigate to="/unauthorized" state={{ from: location }} replace />
  // ) : (
  //   <Navigate to="/login" state={{ from: location }} replace />
  // );

  return allowedRoles?.includes(auth?.role) ? <Outlet /> : <Outlet />;
};

export default RequireAuth;
