import { useState } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { FaBars, FaTimes } from "react-icons/fa";

function Navbar() {
    const { isAuthenticated, logout } = useAuth();
    const [isOpen, setIsOpen] = useState(false);

    const handleLogout = async () => {
        try {
            await logout(); // Llamar al método logout del contexto para actualizar el estado
        } catch (error) {
            console.error("Error logging out:", error);
        }
    };

    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };

    return (
        <nav className="bg-primary text-lightblue my-3 flex justify-between py-5 px-10 items-center">
            <Link to="/" className="text-2xl font-bold text-blue">
                Comunicación
            </Link>
            <div className="md:hidden">
                {isOpen ? (
                    <FaTimes className="text-3xl text-lightblue cursor-pointer" onClick={toggleMenu} />
                ) : (
                    <FaBars className="text-3xl text-lightblue cursor-pointer" onClick={toggleMenu} />
                )}
            </div>
            <ul className={`md:flex space-x-4 ${isOpen ? 'block' : 'hidden'} md:flex`}>
                {isAuthenticated ? (
                    <>
                        <li className="text-lightblue">
                            Bienvenido Usuario
                        </li>
                        <li>
                            <Link to="/wall" className="px-4 py-2 rounded-lg bg-lightblue text-primary font-semibold hover:bg-blue hover:text-white transition duration-300">
                                Wall
                            </Link>
                        </li>
                        <li>
                            <Link to="/perfil" className="px-4 py-2 rounded-lg bg-lightblue text-primary font-semibold hover:bg-blue hover:text-white transition duration-300">
                                Perfil
                            </Link>
                        </li>
                        <li>
                            <button
                                onClick={handleLogout}
                                className="px-4 py-2 rounded-lg bg-lightblue text-primary font-semibold hover:bg-blue hover:text-white transition duration-300"
                            >
                                Logout
                            </button>
                        </li>
                    </>
                ) : (
                    <>
                        <li>
                            <Link to="/Registrar" className="px-4 py-2 rounded-lg bg-lightblue text-primary font-semibold hover:bg-blue hover:text-white transition duration-300">
                                Registrarse
                            </Link>
                        </li>
                        <li>
                            <Link to="/Ingresar" className="px-4 py-2 rounded-lg bg-lightblue text-primary font-semibold hover:bg-blue hover:text-white transition duration-300">
                                Ingresar
                            </Link>
                        </li>
                    </>
                )}
            </ul>
        </nav>
    );
}

export default Navbar;
