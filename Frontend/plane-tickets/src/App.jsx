import { Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import NoPage from "./pages/NoPage";
import Login from "./pages/Login";
import Layout from "./components/Layout";
import RequireAuth from "./components/RequireAuth";
import NewFlight from "./pages/NewFlight";
import NewUser from "./pages/NewUser";
import Flights from "./pages/Flights";
import Reservations from "./pages/Reservations";

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route path="" element={<Home />} />
        <Route path="login" element={<Login />} />
        <Route path="home" element={<Home />} />
        <Route element={<RequireAuth allowedRoles={["Agent"]} />}>
          <Route path="newFlight" element={<NewFlight />} />
          <Route path="agentFlights" element={<Flights role="Agent" />} />
          <Route
            path="agentReservations"
            element={<Reservations role="Agent" />}
          />
        </Route>
        <Route element={<RequireAuth allowedRoles={["Visitor"]} />}>
          <Route path="visitorFlights" element={<Flights role="Visitor" />} />
          <Route
            path="visitorReservations"
            element={<Reservations role="Visitor" />}
          />
        </Route>
        <Route element={<RequireAuth allowedRoles={["Admin"]} />}>
          <Route path="adminFlights" element={<Flights role="Admin" />} />
          <Route path="newUser" element={<NewUser />} />
        </Route>
        <Route path="*" element={<NoPage />} />
      </Route>
    </Routes>
  );
}

export default App;
