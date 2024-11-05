
export function Footer() {

    return (
        <div className="mt-auto">
            <hr className="m-0 mx-4 light" style={{ borderColor: "#b7b7b7" }} />
            <div className="p-2 d-flex justify-content-between px-5">
                <p className="m-0 text-muted">
                    <img src="/images/chip.svg" alt="logo" className="me-2" style={{
                        height: '1rem',
                    }} />
                    Example
                </p>
                <div>
                    <a href="mailto:X@Y.Z" className="text-muted me-2">
                        <i className="fa-solid fa-envelope me-2"></i>
                        Contact Us
                    </a>
                </div>
            </div>
        </div>
    );
}
