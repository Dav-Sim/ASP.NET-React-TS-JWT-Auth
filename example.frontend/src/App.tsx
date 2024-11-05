import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom";
import { HomePage } from "./pages/home/HomePage";
import { Header } from "./components/layout/Header";
import { MainContainer } from "./components/layout/MainContainer";
import { Footer } from "./components/layout/Footer";
import { LoginPage } from "./pages/login/LoginPage";
import { RegisterPage } from "./pages/register/RegisterPage";
import { NotAuthorizedPage } from "./pages/notAuthorized/NotAuthorizedPage";
import { Toaster } from "react-hot-toast";
import { ProtectedRoute } from "./components/Router/ProtectedRoute";
import { VerifyEmailPage } from "./pages/verifyEmail/VerifyEmailPage";
import { UserProfilePage } from "./pages/userProfile/UserProfilePage";
import { UsersPage } from "./pages/users/UsersPage";
import { ForgotPasswordPage } from "./pages/forgotPassword/ForgotPasswordPage";
import { ResetPasswordPage } from "./pages/resetPassword/ResetPasswordPage";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 0, //retry unsuccessful fetch
      gcTime: 0, //how long are data stored in memory
      staleTime: 0, //how long are data fresh
      enabled: true, //fetch automatically
      refetchOnWindowFocus: false,
      refetchOnReconnect: false,
      refetchOnMount: false,
      refetchInterval: 0, //do not refetch automatically
      refetchIntervalInBackground: false,
    },
  }
});

export function App() {
  return (
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <Routes>
          <Route Component={Layout}>
            <Route path={`/`} element={
              <ProtectedRoute>
                <HomePage />
              </ProtectedRoute>}>
            </Route>
            <Route path={`/login`} element={<LoginPage />}></Route>
            <Route path={`/register`} element={<RegisterPage />}></Route>
            <Route path={`/notauthorized`} element={<NotAuthorizedPage />}></Route>
            <Route path={`/verifyemail`} element={<VerifyEmailPage />}></Route>
            <Route path={`/forgotpassword`} element={<ForgotPasswordPage />}></Route>
            <Route path={`/resetpassword`} element={<ResetPasswordPage />}></Route>
            <Route path={`/userprofile`} element={
              <ProtectedRoute>
                <UserProfilePage />
              </ProtectedRoute>
            }></Route>
            <Route path={`/admin/users`} element={
              <ProtectedRoute>
                <UsersPage />
              </ProtectedRoute>
            }></Route>
          </Route>
        </Routes>
      </QueryClientProvider>
    </BrowserRouter>
  )
}

export function Layout() {
  return (
    <div className="d-flex flex-column bg-dim" style={{ minHeight: '100vh' }}>
      <Header />
      <MainContainer>
        <Outlet />
      </MainContainer>
      <Footer />
      <Toaster />
    </div>
  );
}