// Слот 0 - SpriteBatch подставит сюда основную текстуру
sampler TextureSampler : register(s0);
// Слот 1 - Сюда мы вручную "запихнем" карту нормалей
texture NormalMap;
sampler NormalMapSampler : register(s1)
{
    Texture = <NormalMap>;
    // Принудительно отключаем мип-мапы и сглаживание для четкости на 16x34
    MagFilter = Point;
    MinFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

float3 LightDirection;
float3 LightColor; // Сделаем поярче
float3 AmbientColor;

float3 TopColor = float3(1.0, 1.0, 0.9); // Цвет верха (светлый)
float3 BottomColor = float3(0.4, 0.3, 0.2); // Цвет низа (тень)

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    //float4 tex = tex2D(TextureSampler, texCoord);
    //if (tex.a < 0.1)
    //    discard;

    //// 1. ПАРАМЕТРЫ (Подставь свои реальные размеры атласа!)
    //float atlasHeight = 1024.0; // Общая высота картинки с шрифтом
    //float tileHeight = 34.0; // Высота одной строки/символа

    //// 2. ГРАДИЕНТ (Белый -> Светло-серый)
    //float pixelY = texCoord.y * atlasHeight;
    //float localY = (pixelY % tileHeight) / tileHeight;
    
    //// Делаем мягкий переход от чисто белого к сероватому
    //float3 top = float3(1.0, 1.0, 1.0);
    //float3 bot = float3(0.6, 0.6, 0.6);
    //float3 faceColor = lerp(top, bot, localY);

    //// 3. ОБЪЕМНАЯ ФАСКА (Bevel)
    //// Шаг в 1 пиксель вверх
    //float above = tex2D(TextureSampler, texCoord - float2(0, 1.0 / atlasHeight)).a;
    
    //// Если над текущим пикселем пустота — рисуем яркий блик на верхней грани
    //float3 finalColor = faceColor;
    //if (tex.a > above)
    //{
    //    finalColor += 0.4; // Добавляет "белую искру" на верхний край
    //}

    //// Умножаем на color из SpriteBatch.Draw (чтобы работала тонировка)
    //return float4(tex.rgb * finalColor, tex.a) * color;
    
    float4 tex = tex2D(TextureSampler, texCoord);
    if (tex.a < 0.1)
        discard;

    // ВАЖНО: Подставь сюда реальную высоту своей картинки-атласа!
    float atlasHeight = 1024.0;
    float tileHeight = 34.0;

    // Считаем градиент без муара
    float pixelY = texCoord.y * atlasHeight;
    float localY = (pixelY % tileHeight) / tileHeight;
    
    // Мягкий фэнтези-градиент (золотистый/бронзовый)
    float3 top = float3(1.0, 0.9, 0.5);
    float3 bot = float3(0.4, 0.2, 0.1);
    //float3 top = float3(1.0, 0.9, 0.4);
    //float3 bot = float3(0.6, 0.4, 0.1);
    //float3 top = float3(0.95, 0.92, 0.82); // Светлый кремово-золотистый
    //float3 bot = float3(0.55, 0.48, 0.38); // Приглушенный серо-коричневый
    //float3 top = float3(0.85, 0.75, 0.45); // Приглушенный золотой
    //float3 bot = float3(0.35, 0.18, 0.08); // Глубокий коньячный/коричневый
    float3 finalColor = lerp(top, bot, localY);

    // Добавим тонкую светлую каемку сверху для объема (Bevel)
    float above = tex2D(TextureSampler, texCoord - float2(0, 1.0 / atlasHeight)).a;
    if (tex.a > above)
        finalColor += 0.3; // Блик на верхней грани

    return float4(tex.rgb * finalColor, tex.a) * color;
    
    //3D
    
    //float4 tex = tex2D(TextureSampler, texCoord);
    //if (tex.a < 0.1)
    //    discard;

    //// Шаг выборки — сделаем его чуть больше, чтобы захватить края
    //float2 d = float2(1.5 / 2048.0, 1.5 / 2048.0); // Используй размер ВСЕГО атласа здесь

    //// Собираем "высоту" из альфа-канала в нескольких точках (Cross-pattern)
    //float l = tex2D(TextureSampler, texCoord + float2(-d.x, 0)).a;
    //float r = tex2D(TextureSampler, texCoord + float2(d.x, 0)).a;
    //float u = tex2D(TextureSampler, texCoord + float2(0, -d.y)).a;
    //float dn = tex2D(TextureSampler, texCoord + float2(0, d.y)).a;

    //// Генерируем нормаль. 
    //// Коэффициент 2.0 перед (l-r) делает грани круче.
    //// Z = 0.2 делает буквы ОЧЕНЬ выпуклыми.
    //float3 normal = normalize(float3((l - r) * 2.0, (u - dn) * 2.0, 0.2));

    //// СВЕТ (Сделаем его ОСТРЫМ и контрастным)
    //float3 L = normalize(float3(1.0, -1.0, -0.5)); // Свет сбоку-сверху
    
    //// Диффуз + Specular (блик даст 3D эффект)
    //float diff = saturate(dot(normal, L));
    //float spec = pow(saturate(reflect(-L, normal).z), 20.0);

    //// Уменьшаем Ambient, чтобы тени были глубокими
    //float3 finalLight = float3(0.1, 0.1, 0.1) + (diff * float3(1, 1, 1)) + (spec * 0.6);
    
    //return float4(tex.rgb * finalLight, tex.a) * color;
    
}

// Техника (проход)
technique Lighting
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}