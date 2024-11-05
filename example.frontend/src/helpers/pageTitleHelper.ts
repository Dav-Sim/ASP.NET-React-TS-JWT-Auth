import { useEffect } from "react";

export function usePageTitle(title: string) {
    useEffect(() => {
        document.title = `Example - ${title}`;
    }, [title]);
}