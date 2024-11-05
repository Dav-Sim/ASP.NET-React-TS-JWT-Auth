export function MainContainer({
    children
}: Readonly<{
    children: React.ReactNode;
}>) {
    return (
        <main className="flex-fill d-flex justify-content-center p-1 p-md-3">
            <div className="d-flex align-items-center flex-column col-12 shadow-sm p-2 rounded-3 pb-3 mb-3">
                {children}
            </div>
        </main>
    );
}