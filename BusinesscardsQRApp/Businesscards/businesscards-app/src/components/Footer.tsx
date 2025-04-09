import appConfig from "@/lib/appConfig";
const Footer =() =>{
    return(
        <footer className="w-full bg-gray-100 text-gray-600 py-3 px-6 mt-10 shadow-inner">
            <div className="container mx-auto flex justify-between items-center text-sm">
                <div>Version: {appConfig.version}</div>
                <div>{appConfig.copyright}</div>
            </div>
    </footer>
    );    
};

export default Footer;