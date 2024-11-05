import { Link } from "react-router-dom";
import { useAuth, useLogout } from "../../query/authQueries";
import { Loading } from "../loading/Loading";

export function Header() {

    const auth = useAuth();
    const logout = useLogout();

    const handleLogout = (ev: React.SyntheticEvent) => {
        ev.preventDefault();
        logout.mutate();
    }

    return (
        <nav className="nav-gradient text-dark shadow px-1 px-md-2 d-flex justify-content-between align-items-center">
            <div className="d-flex align-items-center justify-content-center">
                <div className="brand py-2 ps-0 ps-md-4">
                    <Link to={`/`} className={`navbar-brand fs-5`}>
                        <img src="/images/chip.svg" alt="logo"
                            className={`me-2`}
                            style={{
                                height: '1.5rem',
                                width: '1.5rem'
                            }} />
                        Example
                    </Link>
                </div>
            </div>

            <Loading loading={auth.isLoading} size="xs" className="d-flex align-items-center justify-content-center">
                <p className={`m-0 p-0`}>
                    {
                        (auth.isLoggedAndVerified || auth.isLoggedButUnverified) ?
                            <i className="fa-solid fa-user me-2"></i>
                            :
                            <i className="fa-solid fa-user-slash me-2"></i>
                    }
                    {
                        auth.isLoggedAndVerified &&
                        <Link to={`/userprofile`} className="me-2">
                            {auth.user?.email}
                        </Link>
                    }
                    {
                        auth.isLoggedButUnverified &&
                        <span className="me-2">
                            {auth.user?.email}
                        </span>
                    }
                    {
                        (auth.isLoggedAndVerified || auth.isLoggedButUnverified) &&
                        <a
                            href="/"
                            onClick={handleLogout}
                        >
                            <i className="fa-solid fa-sign-out me-2"></i>
                        </a>
                    }
                </p>
            </Loading>
        </nav>
    );
}