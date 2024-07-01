import { useEffect, useState } from "react";
import { getFollowers, getFollowing, seguirUsuario, dejarDeSeguirUsuario } from "../api/auth";
import { useAuth } from "../context/AuthContext";
import { Link } from "react-router-dom";

function SeguidoresPages() {
    const { usuario, setSelectedUsername } = useAuth();
    const [followers, setFollowers] = useState([]);
    const [following, setFollowing] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const followersRes = await getFollowers(usuario.usuarioId);
                const followingRes = await getFollowing(usuario.usuarioId);

                

                setFollowers(followersRes);
                setFollowing(followingRes);

                setIsLoading(false);
            } catch (error) {
                console.error("Error fetching followers and following:", error);
                setIsLoading(false);
            }
        };

        if (usuario && usuario.usuarioId) {
            fetchData();
        }
    }, [usuario]);

    const handleFollow = async (followeeId) => {
        try {
            await seguirUsuario(usuario.usuarioId, followeeId);
            const updatedFollowing = await getFollowing(usuario.usuarioId);
            setFollowing(updatedFollowing);
        } catch (error) {
            console.error("Error following user:", error);
        }
    };

    const handleUnfollow = async (followeeId) => {
        try {
            await dejarDeSeguirUsuario(usuario.usuarioId, followeeId);
            const updatedFollowing = await getFollowing(usuario.usuarioId);
            setFollowing(updatedFollowing);
        } catch (error) {
            console.error("Error unfollowing user:", error);
        }
    };

    if (isLoading || !usuario || !usuario.usuarioId) {
        return <div>Loading...</div>;
    }

    return (
        <div className="bg-primary min-h-screen text-lightblue p-4">
            <h1 className="text-4xl font-bold mb-6 text-center text-blue">Seguidores y Seguidos</h1>

            <div className="mb-6">
                <h2 className="text-2xl font-semibold mb-4 text-blue">Seguidores</h2>
                <ul className="space-y-2">
                    {followers.map((follower) => (
                        <li key={follower.usuarioId} className="flex justify-between items-center">
                            <Link
                                to={`/perfilUsuario`}
                                className="text-lightblue"
                                onClick={() => setSelectedUsername(follower.usuario)}
                            ><span className="text-lightblue">{follower.usuario}</span></Link>
                            {following.some((user) => user.usuarioId === follower.usuarioId) ? (
                                <button
                                    className="bg-red-500 text-primary py-1 px-4 rounded hover:bg-red-700 transition duration-300"
                                    onClick={() => handleUnfollow(follower.usuarioId)}
                                >
                                    Dejar de seguir
                                </button>
                            ) : (
                                <button
                                    className="bg-blue text-primary py-1 px-4 rounded hover:bg-lightblue transition duration-300"
                                    onClick={() => handleFollow(follower.usuarioId)}
                                >
                                    Seguir
                                </button>
                            )}
                        </li>
                    ))}
                </ul>
            </div>

            <div>
                <h2 className="text-2xl font-semibold mb-4 text-blue">Seguidos</h2>
                <ul className="space-y-2">
                    {following.map((followee) => (
                        <li key={followee.usuarioId} className="flex justify-between items-center">
                            <Link 
                                to={`/perfilUsuario`}
                                className="text-lightblue"
                                onClick={() => setSelectedUsername(followee.usuario)}
                            ><span className="text-lightblue">{followee.usuario}</span></Link>
                            <button
                                className="bg-red-500 text-primary py-1 px-4 rounded hover:bg-red-700 transition duration-300"
                                onClick={() => handleUnfollow(followee.usuarioId)}
                            >
                                Dejar de seguir
                            </button>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
}

export default SeguidoresPages;
