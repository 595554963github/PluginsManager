<Window x:Class="PluginManagerWPF.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="插件管理器" Height="600" Width="900"
        WindowStartupLocation="CenterScreen"
        Background="#2D2D30">
    <Window.Resources>
        <Style x:Key="DataGridTextStyle" TargetType="TextBlock">
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="VerticalAlignment" Value="Center"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="TextWrapping" Value="Wrap"/>
        </Style>
        <Style x:Key="CustomToolTipStyle" TargetType="ToolTip">
            <Setter Property="Background" Value="#2D2D30"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderBrush" Value="#4EC9B0"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="Padding" Value="8"/>
            <Setter Property="FontSize" Value="12"/>
        </Style>
        <Style x:Key="CustomScrollBar" TargetType="ScrollBar">
            <Setter Property="Stylus.IsPressAndHoldEnabled" Value="false"/>
            <Setter Property="Stylus.IsFlicksEnabled" Value="false"/>
            <Setter Property="Background" Value="#1E1E1E"/>
            <Setter Property="BorderBrush" Value="#1E1E1E"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Width" Value="14"/>
            <Setter Property="MinWidth" Value="14"/>
            <Setter Property="Visibility" Value="Visible"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="ScrollBar">
                        <Grid x:Name="Bg" Background="{TemplateBinding Background}" SnapsToDevicePixels="true">
                            <Grid.RowDefinitions>
                                <RowDefinition MaxHeight="18"/>
                                <RowDefinition Height="*"/>
                                <RowDefinition MaxHeight="18"/>
                            </Grid.RowDefinitions>

                            <RepeatButton x:Name="PART_LineUpButton" 
                                  Grid.Row="0"
                                  Command="ScrollBar.LineUpCommand"
                                  Background="#1E1E1E"
                                  BorderBrush="#4EC9B0"
                                  BorderThickness="1"
                                  IsEnabled="{TemplateBinding IsEnabled}">
                                <Path Fill="White" Data="M 0,4 L 4,0 L 8,4 Z" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                            </RepeatButton>

                            <Track x:Name="PART_Track" Grid.Row="1" IsDirectionReversed="true" IsEnabled="{TemplateBinding IsEnabled}">
                                <Track.DecreaseRepeatButton>
                                    <RepeatButton Command="ScrollBar.PageUpCommand" Background="Transparent" BorderThickness="0"/>
                                </Track.DecreaseRepeatButton>
                                <Track.Thumb>
                                    <Thumb Background="#4EC9B0" BorderBrush="#3CB371" BorderThickness="1" Margin="1,0,1,0">
                                        <Thumb.Template>
                                            <ControlTemplate TargetType="Thumb">
                                                <Border Background="{TemplateBinding Background}" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" CornerRadius="2"/>
                                            </ControlTemplate>
                                        </Thumb.Template>
                                    </Thumb>
                                </Track.Thumb>
                                <Track.IncreaseRepeatButton>
                                    <RepeatButton Command="ScrollBar.PageDownCommand" Background="Transparent" BorderThickness="0"/>
                                </Track.IncreaseRepeatButton>
                            </Track>

                            <RepeatButton x:Name="PART_LineDownButton" 
                                  Grid.Row="2"
                                  Command="ScrollBar.LineDownCommand"
                                  Background="#1E1E1E"
                                  BorderBrush="#4EC9B0"
                                  BorderThickness="1"
                                  IsEnabled="{TemplateBinding IsEnabled}">
                                <Path Fill="White" Data="M 0,0 L 4,4 L 8,0 Z" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                            </RepeatButton>
                        </Grid>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="CustomDataGridStyle" TargetType="DataGrid">
            <Setter Property="Background" Value="#1E1E1E"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderBrush" Value="#4EC9B0"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="RowDetailsVisibilityMode" Value="VisibleWhenSelected"/>
            <Setter Property="ScrollViewer.CanContentScroll" Value="true"/>
            <Setter Property="ScrollViewer.PanningMode" Value="Both"/>
            <Setter Property="Stylus.IsFlicksEnabled" Value="False"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGrid">
                        <Border BorderBrush="{TemplateBinding BorderBrush}" 
                        BorderThickness="{TemplateBinding BorderThickness}" 
                        Background="{TemplateBinding Background}" 
                        Padding="{TemplateBinding Padding}" 
                        SnapsToDevicePixels="True">
                            <ScrollViewer x:Name="DG_ScrollViewer" 
                                  Focusable="false"
                                  VerticalScrollBarVisibility="Visible"
                                  HorizontalScrollBarVisibility="Disabled">
                                <ScrollViewer.Resources>
                                    <Style TargetType="ScrollBar" BasedOn="{StaticResource CustomScrollBar}"/>
                                </ScrollViewer.Resources>
                                <ScrollViewer.Content>
                                    <ItemsPresenter SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}"/>
                                </ScrollViewer.Content>
                            </ScrollViewer>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="StatusTextStyle" TargetType="TextBlock" BasedOn="{StaticResource DataGridTextStyle}">
            <Setter Property="Foreground" Value="#FF6B6B"/>
            <Setter Property="Text" Value="未下载"/>
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsDownloaded}" Value="True">
                    <Setter Property="Text" Value="已下载"/>
                    <Setter Property="Foreground" Value="#008000"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>

        <Style x:Key="DataGridRowStyle" TargetType="DataGridRow">
            <Setter Property="Background" Value="Transparent"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0,0,0,1"/>
            <Setter Property="BorderBrush" Value="#4EC9B0"/>
            <Setter Property="ToolTip" Value="{Binding ToolTip}"/>
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter Property="Background" Value="#3A3A3A"/>
                </Trigger>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="#4EC9B0"/>
                    <Setter Property="Foreground" Value="White"/>
                </Trigger>
            </Style.Triggers>
        </Style>

        <Style x:Key="DataGridCellStyle" TargetType="DataGridCell">
            <Setter Property="Background" Value="Transparent"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="1,0,1,0"/>
            <Setter Property="BorderBrush" Value="#3CB371"/>
            <Setter Property="Padding" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridCell">
                        <Border BorderBrush="{TemplateBinding BorderBrush}" 
                                BorderThickness="{TemplateBinding BorderThickness}" 
                                Background="{TemplateBinding Background}" 
                                SnapsToDevicePixels="True">
                            <ContentPresenter SnapsToDevicePixels="{TemplateBinding SnapsToDevicePixels}" 
                                            VerticalAlignment="Center"
                                            Margin="8,4"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="DataGridColumnHeaderStyle" TargetType="DataGridColumnHeader">
            <Setter Property="Background" Value="#2D2D30"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="BorderBrush" Value="#4EC9B0"/>
            <Setter Property="HorizontalContentAlignment" Value="Left"/>
            <Setter Property="Padding" Value="8,4"/>
            <Setter Property="FontWeight" Value="Bold"/>
        </Style>
    </Window.Resources>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <DataGrid x:Name="PluginsDataGrid" 
                  Grid.Row="0"
                  Margin="10"
                  Style="{StaticResource CustomDataGridStyle}"
                  AutoGenerateColumns="False"
                  HeadersVisibility="Column"
                  RowHeaderWidth="0"
                  GridLinesVisibility="None"
                  SelectionMode="Single"
                  SelectionUnit="FullRow"
                  IsReadOnly="True"
                  RowStyle="{StaticResource DataGridRowStyle}"
                  CellStyle="{StaticResource DataGridCellStyle}">

            <DataGrid.Columns>
                <DataGridTextColumn Header="插件名称" 
                                   Binding="{Binding DisplayName}" 
                                   Width="*"
                                   HeaderStyle="{StaticResource DataGridColumnHeaderStyle}"
                                   ElementStyle="{StaticResource DataGridTextStyle}"/>

                <DataGridTemplateColumn Header="状态" 
                                       Width="*"
                                       HeaderStyle="{StaticResource DataGridColumnHeaderStyle}">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <TextBlock Style="{StaticResource StatusTextStyle}"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>

                <DataGridTextColumn Header="文件" 
                                   Binding="{Binding FileName}" 
                                   Width="*"
                                   HeaderStyle="{StaticResource DataGridColumnHeaderStyle}"
                                   ElementStyle="{StaticResource DataGridTextStyle}"/>

                <DataGridTextColumn Header="类型" 
                                   Binding="{Binding PluginType}" 
                                   Width="*"
                                   HeaderStyle="{StaticResource DataGridColumnHeaderStyle}"
                                   ElementStyle="{StaticResource DataGridTextStyle}"/>
            </DataGrid.Columns>
        </DataGrid>
        <Border Grid.Row="1" Background="#2D2D30" Padding="10,5">
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                <TextBlock Text="搜索|过滤" Foreground="White" VerticalAlignment="Center" Margin="0,0,5,0"/>
                <TextBox x:Name="SearchTextBox" Width="150" Height="25" Padding="5" Margin="0,0,20,0"
                         Background="#1E1E1E" Foreground="White" BorderBrush="#4EC9B0"
                         TextChanged="SearchTextBox_TextChanged">
                    <TextBox.ToolTip>
                        <ToolTip Content="输入插件名称进行过滤，如果将来添加的插件更多了，甚至有名称接近的，那你可以用它来筛选" Style="{StaticResource CustomToolTipStyle}"/>
                    </TextBox.ToolTip>
                </TextBox>

                <Button x:Name="DownloadButton" Content="下载" Width="80" Height="30" Margin="5" 
                        Background="#0E639C" Foreground="White" BorderBrush="#0E639C"
                        Click="DownloadButton_Click">
                    <Button.ToolTip>
                        <ToolTip Content="点击此按钮下载选择的插件" Style="{StaticResource CustomToolTipStyle}"/>
                    </Button.ToolTip>
                </Button>

                <Button x:Name="LaunchButton" Content="启动" Width="80" Height="30" Margin="5" 
                        Background="#0E639C" Foreground="White" BorderBrush="#0E639C"
                        Click="LaunchButton_Click">
                    <Button.ToolTip>
                        <ToolTip Content="启动已下载的插件" Style="{StaticResource CustomToolTipStyle}"/>
                    </Button.ToolTip>
                </Button>

                <Button x:Name="UninstallButton" Content="卸载" Width="80" Height="30" Margin="5" 
                        Background="#0E639C" Foreground="White" BorderBrush="#0E639C"
                        Click="UninstallButton_Click">
                    <Button.ToolTip>
                        <ToolTip Content="卸载已下载的插件" Style="{StaticResource CustomToolTipStyle}"/>
                    </Button.ToolTip>
                </Button>

                <Button x:Name="SortButton" Content="排序↑" Width="80" Height="30" Margin="5" 
                        Background="#0E639C" Foreground="White" BorderBrush="#0E639C"
                        Click="SortButton_Click">
                    <Button.ToolTip>
                        <ToolTip Content="点击此按钮给插件按名称排序" Style="{StaticResource CustomToolTipStyle}"/>
                    </Button.ToolTip>
                </Button>
            </StackPanel>
        </Border>
    </Grid>
</Window>
