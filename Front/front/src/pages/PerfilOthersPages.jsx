import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getUsuarioPosts, seguirUsuario, dejarDeSeguirUsuario, getUserByUsername, checkIfFollowing, getFollowers, getFollowing } from "../api/auth";

function PerfilOthersPages() {
    const { usuario, selectedUsername } = useAuth(); // Obtener el nombre de usuario actual y el nombre de usuario seleccionado del contexto de autenticación

    const [userInfo, setUserInfo] = useState({
        usuario: selectedUsername,
        seguidores: 0,
        siguiendo: 0,
    });
    const [userPosts, setUserPosts] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isFollowing, setIsFollowing] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                // Obtener información del usuario por username seleccionado
                const userData = await getUserByUsername(selectedUsername);

                // Obtener seguidores y seguidos
                const followersRes = await getFollowers(userData.usuarioId);
                const followingRes = await getFollowing(userData.usuarioId);

                setUserInfo({
                    usuario: userData.usuario,
                    seguidores: followersRes.length,
                    siguiendo: followingRes.length,
                });

                // Obtener los posts del usuario usando userData.usuarioId
                const userPostsRes = await getUsuarioPosts(userData.usuarioId);
                setUserPosts(userPostsRes);

                // Verificar si el usuario actual sigue a este usuario
                const isFollowingRes = await checkIfFollowing(usuario.usuarioId, userData.usuarioId);
                setIsFollowing(isFollowingRes);

                setIsLoading(false);
            } catch (error) {
                console.error("Error fetching data:", error);
                setIsLoading(false);
            }
        };

        if (selectedUsername) {
            fetchData();
        }
    }, [selectedUsername, usuario.usuarioId]);

    const handleFollowUser = async () => {
        try {
            await seguirUsuario(usuario.usuarioId, userInfo.usuarioId); // Seguir al usuario seleccionado
            setIsFollowing(true);
            // Actualizar el número de seguidores localmente
            setUserInfo((prevUserInfo) => ({
                ...prevUserInfo,
                seguidores: prevUserInfo.seguidores + 1,
            }));
        } catch (error) {
            console.error("Error following user:", error);
        }
    };

    const handleUnfollowUser = async () => {
        try {
            await dejarDeSeguirUsuario(usuario.usuarioId, userInfo.usuarioId); // Dejar de seguir al usuario seleccionado
            setIsFollowing(false);
            // Actualizar el número de seguidores localmente
            setUserInfo((prevUserInfo) => ({
                ...prevUserInfo,
                seguidores: prevUserInfo.seguidores - 1,
            }));
        } catch (error) {
            console.error("Error unfollowing user:", error);
        }
    };

    const formatDate = (timestamp) => {
        const date = new Date(timestamp);
        return date.toLocaleString(); // Formato localizado de fecha y hora
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="bg-primary min-h-screen text-lightblue p-4">
            <h1 className="text-4xl font-bold mb-6 text-center text-blue">Perfil de {userInfo.usuario}</h1>
            <div className="flex justify-between items-center mb-6">
                <div>
                    <p className="text-xl font-semibold">Número de posts: {userPosts.length}</p>
                    <p className="text-xl font-semibold">Seguidores: {userInfo.seguidores}</p>
                    <p className="text-xl font-semibold">Siguiendo: {userInfo.siguiendo}</p>
                </div>
                <div>
                    {!isFollowing ? (
                        <button
                            className="bg-blue text-primary py-1 px-4 rounded hover:bg-lightblue transition duration-300"
                            onClick={handleFollowUser}
                        >
                            Seguir
                        </button>
                    ) : (
                        <button
                            className="bg-red-500 text-primary py-1 px-4 rounded hover:bg-red-600 transition duration-300"
                            onClick={handleUnfollowUser}
                        >
                            Dejar de seguir
                        </button>
                    )}
                </div>
            </div>
            <div>
                <h2 className="text-2xl font-semibold mb-4 text-blue">Posts de {userInfo.usuario}</h2>
                {userPosts.length === 0 ? (
                    <p className="text-lightblue">No hay posts.</p>
                ) : (
                    <div className="space-y-4">
                        {userPosts.map((post) => (
                            <div key={post.postId} className="bg-secondary p-4 rounded-lg shadow-md">
                                <p className="text-lightblue">{post.content}</p>
                                <span className="text-sm text-blue">{formatDate(post.timestamp)}</span>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}

export default PerfilOthersPages;
